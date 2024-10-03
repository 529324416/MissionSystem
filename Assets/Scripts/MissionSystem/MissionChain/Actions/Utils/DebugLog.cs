using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace RedSaw.MissionSystem
{
    [Name("Debug Log"), Category("Utils"), Description("Log a message to the console.")]
    public class DebugLog : ActionBase
    {
        [SerializeField] private string message;
        [SerializeField] private LogType logType = LogType.Log;

        public override void Execute()
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Exception:
                    Debug.LogException(new System.Exception(message));
                    break;
            }
        }
        
        
        
#if UNITY_EDITOR

        public override string Summary
        {
            get
            {
                return $"Output: \"{message}\"";
            }
        }
        
        protected override void OnInspectorGUI()
        {
            message = UnityEditor.EditorGUILayout.TextField("Message", message);
            logType = (LogType)UnityEditor.EditorGUILayout.EnumPopup("Log Type", logType);
        }
#endif
    }
}