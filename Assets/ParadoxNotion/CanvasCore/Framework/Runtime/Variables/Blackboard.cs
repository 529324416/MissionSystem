﻿using System;
using System.Collections.Generic;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;
using System.Linq;


namespace NodeCanvas.Framework
{

    ///<summary> A Blackboard component to hold variables</summary>
    [ParadoxNotion.Design.SpoofAOT]
    public class Blackboard : MonoBehaviour, ISerializationCallbackReceiver, IBlackboard
    {

        //Remark: We serialize the whole blackboard as normal which is the previous behaviour.
        //To support prefab overrides we now also serialize each variable individually.
        //Each serialized variable has it's own list of references since it can be an object with multiple
        //UnityObject fields, or simply a list of UnityObjects.
        //We keep both old and new serializations. If something goes wrong or needs change with the new one,
        //there is still the old one to fallback to.

        [Tooltip("An optional Parent Blackboard Asset to 'inherit' variables from.")]
        [SerializeField] private AssetBlackboard _parentBlackboard = null;
        [SerializeField] private string _serializedBlackboard;
        [SerializeField] private List<UnityEngine.Object> _objectReferences;
        [SerializeField] private SerializationPair[] _serializedVariables;

        [NonSerialized] private BlackboardSource _blackboard = new BlackboardSource();
        [NonSerialized] private bool haltForUndo = false;
        [NonSerialized] private string _identifier;

        ///----------------------------------------------------------------------------------------------
        void ISerializationCallbackReceiver.OnBeforeSerialize() { SelfSerialize(); }
        void ISerializationCallbackReceiver.OnAfterDeserialize() { SelfDeserialize(); }
        ///----------------------------------------------------------------------------------------------

        ///<summary>Self Serialize blackboard</summary>
        public void SelfSerialize() {

#if UNITY_EDITOR
            //This fixes an edge case of cycle referencing prefab blackboards when the Library folder is deleted
            //which was basically due to the prefabs being serialized before the database was re-built.
            if ( UnityEditor.EditorApplication.isUpdating ) {
                return;
            }
#endif

            if ( haltForUndo /*|| ParadoxNotion.Services.Threader.applicationIsPlaying*/ ) {
                return;
            }

            var newReferences = new List<UnityEngine.Object>();
            var newSerialization = JSONSerializer.Serialize(typeof(BlackboardSource), _blackboard, newReferences);
            if ( newSerialization != _serializedBlackboard || !newReferences.SequenceEqual(_objectReferences) || ( _serializedVariables == null || _serializedVariables.Length != _blackboard.variables.Count ) ) {

                haltForUndo = true;
                UndoUtility.RecordObject(this, UndoUtility.GetLastOperationNameOr("Blackboard Change"));
                haltForUndo = false;

                _serializedVariables = new SerializationPair[_blackboard.variables.Count];
                for ( var i = 0; i < _blackboard.variables.Count; i++ ) {
                    var serializedVariable = new SerializationPair();
                    serializedVariable._json = JSONSerializer.Serialize(typeof(Variable), _blackboard.variables.ElementAt(i).Value, serializedVariable._references);
                    _serializedVariables[i] = serializedVariable;
                }

                _serializedBlackboard = newSerialization;
                _objectReferences = newReferences;
            }
        }

        ///<summary>Self Deserialize blackboard</summary>
        public void SelfDeserialize() {

            _blackboard = new BlackboardSource();
            if ( !string.IsNullOrEmpty(_serializedBlackboard) /*&& ( _serializedVariables == null || _serializedVariables.Length == 0 )*/ ) {
                JSONSerializer.TryDeserializeOverwrite<BlackboardSource>(_blackboard, _serializedBlackboard, _objectReferences);
            }

            //this is to handle prefab overrides
            if ( _serializedVariables != null && _serializedVariables.Length > 0 ) {
                _blackboard.variables.Clear();
                for ( var i = 0; i < _serializedVariables.Length; i++ ) {
                    var variable = JSONSerializer.Deserialize<Variable>(_serializedVariables[i]._json, _serializedVariables[i]._references);
                    _blackboard.variables[variable.name] = variable;
                }
            }
        }

        ///<summary>Serialize the blackboard to json with optional list to store object references within. Use this in runtime for blackboard save/load</summary>
        public string Serialize(List<UnityEngine.Object> references, bool pretyJson = false) {
            return JSONSerializer.Serialize(typeof(BlackboardSource), _blackboard, references, pretyJson);
        }

        ///<summary>Deserialize the blackboard from json with optional list of object references to read serializedreferences from. We deserialize ON TOP of existing variables so that external references to them stay intact. Use this in runtime for blackboard save/load</summary>
        public bool Deserialize(string json, List<UnityEngine.Object> references, bool removeMissingVariables = true) {
            var deserializedBB = JSONSerializer.Deserialize<BlackboardSource>(json, references);
            if ( deserializedBB == null ) { return false; }
            this.OverwriteFrom(deserializedBB, removeMissingVariables);
            this.InitializePropertiesBinding(( (IBlackboard)this ).propertiesBindTarget, true);
            return true;
        }

        ///----------------------------------------------------------------------------------------------

        public event System.Action<Variable> onVariableAdded;
        public event System.Action<Variable> onVariableRemoved;

        string IBlackboard.identifier => _identifier;
        Dictionary<string, Variable> IBlackboard.variables { get { return _blackboard.variables; } set { _blackboard.variables = value; } }
        Component IBlackboard.propertiesBindTarget => this;
        UnityEngine.Object IBlackboard.unityContextObject => this;
        IBlackboard IBlackboard.parent => _parentBlackboard;
        string IBlackboard.independantVariablesFieldName => nameof(_serializedVariables);

        void IBlackboard.TryInvokeOnVariableAdded(Variable variable) { if ( onVariableAdded != null ) onVariableAdded(variable); }
        void IBlackboard.TryInvokeOnVariableRemoved(Variable variable) { if ( onVariableRemoved != null ) onVariableRemoved(variable); }

        ///----------------------------------------------------------------------------------------------

        //...
        virtual protected void Awake() {
            _identifier = gameObject.name;
            this.InitializePropertiesBinding(( (IBlackboard)this ).propertiesBindTarget, false);
        }

        ///----------------------------------------------------------------------------------------------

        //These exist here only for backward compatibility in case ppl used these methods in any reflection

        ///<summary>Add a new variable of name and type</summary>
        public Variable AddVariable(string name, System.Type type) { return IBlackboardExtensions.AddVariable(this, name, type); }
        ///<summary>Add a new variable of name and value</summary>
        public Variable AddVariable(string name, object value) { return IBlackboardExtensions.AddVariable(this, name, value); }
        ///<summary>Delete the variable with specified name</summary>
        public Variable RemoveVariable(string name) { return IBlackboardExtensions.RemoveVariable(this, name); }
        ///<summary>Get a Variable of name and optionaly type</summary>
        public Variable GetVariable(string name, System.Type ofType = null) { return IBlackboardExtensions.GetVariable(this, name, ofType); }
        ///<summary>Get a Variable of ID and optionaly type</summary>
        public Variable GetVariableByID(string ID) { return IBlackboardExtensions.GetVariableByID(this, ID); }
        //Generic version of get variable
        public Variable<T> GetVariable<T>(string name) { return IBlackboardExtensions.GetVariable<T>(this, name); }
        ///<summary>Get the variable value of name</summary>
        public T GetVariableValue<T>(string name) { return IBlackboardExtensions.GetVariableValue<T>(this, name); }
        ///<summary>Set the variable value of name</summary>
        public Variable SetVariableValue(string name, object value) { return IBlackboardExtensions.SetVariableValue(this, name, value); }

        [System.Obsolete("Use GetVariableValue")]
        public T GetValue<T>(string name) { return GetVariableValue<T>(name); }
        [System.Obsolete("Use SetVariableValue")]
        public Variable SetValue(string name, object value) { return SetVariableValue(name, value); }

        ///----------------------------------------------------------------------------------------------

        [ContextMenu("Show Json")]
        void ShowJson() { JSONSerializer.ShowData(_serializedBlackboard, this.name); }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Saves the Blackboard in PlayerPrefs with saveKey being it's name. You can use this for a Save system</summary>
        public string Save() { return Save(this.name); }
        ///<summary>Saves the Blackboard in PlayerPrefs in the provided saveKey. You can use this for a Save system</summary>
        public string Save(string saveKey) {
            var json = Serialize(null);
            PlayerPrefs.SetString(saveKey, json);
            return json;
        }

        ///<summary>Loads back the Blackboard from PlayerPrefs saveKey same as it's name. You can use this for a Save system</summary>
        public bool Load() { return Load(this.name); }
        ///<summary>Loads back the Blackboard from PlayerPrefs of the provided saveKey. You can use this for a Save system</summary>
        public bool Load(string saveKey) {
            var json = PlayerPrefs.GetString(saveKey);
            if ( string.IsNullOrEmpty(json) ) {
                Debug.Log("No data to load blackboard variables from key " + saveKey);
                return false;
            }
            return Deserialize(json, null, true);
        }

        ///----------------------------------------------------------------------------------------------

        virtual protected void OnValidate() {
            _identifier = gameObject.name;
            // if ( UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this) ) {
            //     var serializedContext = new UnityEditor.SerializedObject(this);
            //     var variablesProperty = serializedContext.FindProperty(nameof(_serializedVariables));
            //     var prefabAssetPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this);
            //     var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<Blackboard>(prefabAssetPath);
            //     for ( var i = 0; i < _serializedVariables.Length; i++ ) {
            //         if ( i >= prefab._serializedVariables.Length ) { break; }
            //         var varProp = variablesProperty.GetArrayElementAtIndex(i);
            //         var instVariable = JSONSerializer.Deserialize<Variable>(_serializedVariables[i]._json);
            //         var prefVariable = JSONSerializer.Deserialize<Variable>(prefab._serializedVariables[i]._json);
            //     }
            // }
        }

        public override string ToString() { return _identifier; }
    }
}