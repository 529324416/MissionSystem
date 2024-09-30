﻿using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using System.Linq;


namespace NodeCanvas.Tasks.Actions
{

    [Name("Set Property (Desktop Only)", 7)]
    [Category("✫ Reflected/Faster Versions (Desktop Platforms Only)")]
    [Description("This version works in destop/JIT platform only.\n\nSet a property on a script.")]
    public class SetProperty : ActionTask, IReflectedWrapper
    {

        [SerializeField]
        protected ReflectedActionWrapper functionWrapper;

        private MethodInfo targetMethod { get { return functionWrapper != null ? functionWrapper.GetMethod() : null; } }

        public override System.Type agentType {
            get
            {
                if ( targetMethod == null ) { return typeof(Transform); }
                return targetMethod.IsStatic ? null : targetMethod.RTReflectedOrDeclaredType();
            }
        }

        protected override string info {
            get
            {
                if ( functionWrapper == null ) { return "No Property Selected"; }
                if ( targetMethod == null ) { return functionWrapper.AsString().FormatError(); }
                var mInfo = targetMethod.IsStatic ? targetMethod.RTReflectedOrDeclaredType().FriendlyName() : agentInfo;
                return string.Format("{0}.{1} = {2}", mInfo, targetMethod.Name, functionWrapper.GetVariables()[0]);
            }
        }

        ParadoxNotion.Serialization.ISerializedReflectedInfo IReflectedWrapper.GetSerializedInfo() { return functionWrapper?.GetSerializedMethod(); }

        public override void OnValidate(ITaskSystem ownerSystem) {
            if ( functionWrapper != null && functionWrapper.HasChanged() ) {
                SetMethod(functionWrapper.GetMethod());
            }
        }

        //store the method info on init for performance
        protected override string OnInit() {
            if ( targetMethod == null ) { return "Missing Property"; }

            try {
                functionWrapper.Init(targetMethod.IsStatic ? null : agent);
                return null;
            }
            catch { return "SetProperty Error"; }
        }

        //do it by invoking method
        protected override void OnExecute() {

            if ( functionWrapper == null ) {
                EndAction(false);
                return;
            }

            functionWrapper.Call();
            EndAction();
        }

        void SetMethod(MethodInfo method) {
            if ( method != null ) {
                UndoUtility.RecordObject(ownerSystem.contextObject, "Set Reflection Member");
                functionWrapper = ReflectedActionWrapper.Create(method, blackboard);
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( !Application.isPlaying && GUILayout.Button("Select Property") ) {
                var menu = new UnityEditor.GenericMenu();
                if ( agent != null ) {
                    foreach ( var comp in agent.GetComponents(typeof(Component)).Where(c => !c.hideFlags.HasFlag(HideFlags.HideInInspector)) ) {
                        menu = EditorUtils.GetInstanceMethodSelectionMenu(comp.GetType(), typeof(void), typeof(object), SetMethod, 1, true, false, menu);
                    }
                    menu.AddSeparator("/");
                }
                foreach ( var t in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                    menu = EditorUtils.GetStaticMethodSelectionMenu(t, typeof(void), typeof(object), SetMethod, 1, true, false, menu);
                    if ( typeof(UnityEngine.Component).IsAssignableFrom(t) ) {
                        menu = EditorUtils.GetInstanceMethodSelectionMenu(t, typeof(void), typeof(object), SetMethod, 1, true, false, menu);
                    }
                }
                menu.ShowAsBrowser("Select Property", this.GetType());
                Event.current.Use();
            }

            if ( targetMethod != null ) {
                GUILayout.BeginVertical("box");
                UnityEditor.EditorGUILayout.LabelField("Type", targetMethod.RTReflectedOrDeclaredType().FriendlyName());
                UnityEditor.EditorGUILayout.LabelField("Property", targetMethod.Name);
                UnityEditor.EditorGUILayout.LabelField("Set Type", functionWrapper.GetVariables()[0].varType.FriendlyName());
                UnityEditor.EditorGUILayout.HelpBox(XMLDocs.GetMemberSummary(targetMethod), UnityEditor.MessageType.None);
                GUILayout.EndVertical();
                NodeCanvas.Editor.BBParameterEditor.ParameterField("Set Value", functionWrapper.GetVariables()[0]);
            }
        }

#endif
    }
}