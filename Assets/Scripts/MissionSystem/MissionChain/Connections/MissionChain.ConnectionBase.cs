using UnityEngine;
using NodeCanvas.Framework;
using System;
using ParadoxNotion.Design;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RedSaw.MissionSystem
{
    public class ConnectionBase : Connection 
    { 
        [SerializeField] private bool hasCondition;
        [SerializeField] private ConditionBase _condition;
        
        public bool IsAvailable
        {
            get
            {
                if (!isActive) return false;
                if (!hasCondition || _condition == null) return true;
                return _condition.IsConditionMet;
            }
        }

#if UNITY_EDITOR
        protected override string GetConnectionInfo()
        {
            if(!hasCondition)return string.Empty;
            return _condition == null ? "No Condition" : _condition.Summary;
        }

        protected override void OnConnectionInspectorGUI()
        {
            base.OnConnectionInspectorGUI();
            hasCondition = UnityEditor.EditorGUILayout.Toggle("Has Condition", hasCondition);
            if(!hasCondition)return;
            
            // Draw the condition field
            if(_condition == null)
            {
                if(GUILayout.Button("Add Condition"))
                {
                    Action<Type> OnConditionSelected = (type) =>
                    {
                        UndoUtility.RecordObject(graph, "Condition Added");
                        _condition = (ConditionBase)Activator.CreateInstance(type);
                    };

                    var menu = EditorUtils.GetTypeSelectionMenu(typeof(ConditionBase), OnConditionSelected);
                    menu.ShowAsBrowser("Select Condition");
                }
            }else{
                _condition.DrawInspector();
                if(GUILayout.Button("Remove Condition"))
                {
                    _condition = null;
                }
            }
        }
#endif
    }
}