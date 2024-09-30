using UnityEngine;
using ParadoxNotion.Design;
using ParadoxNotion;

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace RedSaw.MissionSystem
{
    /// <summary>base class of mission actions</summary>
    [System.Serializable]
    public abstract class ActionBase
    {
        
        /// <summary>perform action with current parameters</summary>
        public abstract void Execute();
        
#if UNITY_EDITOR
        public NodeAction _node;
        private string _title;
        private string _summary;

        /// <summary>title of current action</summary>
        public virtual string Title
        {
            get
            {
                _title ??= this.FetchAttribute<NameAttribute>(out var attr)
                    ? attr.name
                    : GetType().Name.SplitCamelCase();
                return _title;
            }
        }

        /// <summary>summary of current action</summary>
        public virtual string Summary
        {
            get
            {
                _summary ??= this.FetchAttribute<DescriptionAttribute>(out var attr)
                    ? $"\"{attr.description}\""
                    : "No Summary Info";
                return _summary;
            }
        }
        
        protected bool _unfolded;

        protected void DrawTitleBar()
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                string _summaryInfo =
                    _unfolded ? "" : "\n" + $"<i><size=11><color=#a9a9a9>{Summary}</color></size></i>";
                GUILayout.Label(
                    "<b>" + (_unfolded ? "▼ " : "► ") + Title + "</b>" + _summaryInfo
                    , Styles.leftLabel);

                /* C# icon, open script while clicked */
                if (GUILayout.Button(Icons.csIcon, GUI.skin.label, GUILayout.Width(20), GUILayout.Height(20)))
                    EditorUtils.OpenScriptOfType(this.GetType());
                    
                /* gear icon, open context menu while clicked */
                if (GUILayout.Button(Icons.gearPopupIcon, Styles.centerLabel, GUILayout.Width(20),
                        GUILayout.Height(20)))
                    GetContextMenu().ShowAsContext();
            }
            GUILayout.EndHorizontal();

            /* unfold control */
            var titleRect = GUILayoutUtility.GetLastRect();
            var e = Event.current;
            if(e.type == EventType.MouseDown && e.button == 0 && titleRect.Contains(e.mousePosition))
            {
                _unfolded = !_unfolded;
                e.Use();
            }
        }
        
        public virtual void DrawInspector()
        {
            DrawTitleBar();
            if (_unfolded)
            {
                GUILayout.BeginVertical("box");
                OnInspectorGUI();
                GUILayout.EndVertical();
            }
        }

        private GenericMenu GetContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Open Script"), false, () => EditorUtils.OpenScriptOfType(this.GetType()));
            menu.AddSeparator("/");
            return OnCreateContextMenu(menu);
        }

        protected virtual GenericMenu OnCreateContextMenu(GenericMenu menu) => menu;

        protected virtual void OnInspectorGUI() { }
#endif
    }
}