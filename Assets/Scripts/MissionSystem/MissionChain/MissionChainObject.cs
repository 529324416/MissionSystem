using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace RedSaw.MissionSystem
{
    [System.Serializable]
    public abstract class MissionChainObject
    {
#if UNITY_EDITOR

        public bool _unfolded;
        private string _title;
        private string _summary;
        private string _description;

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

        public virtual string Description
        {
            get
            {
                _description ??= this.FetchAttribute<DescriptionAttribute>(out var attr)
                    ? attr.description
                    : "No Description Info";
                return _description;
            }
        }

        public virtual void DrawInspector()
        {
            DrawTitleBar();
            if (_unfolded)
            {
                OnInspectorGUI();
            }
        }

        protected virtual string TitleBarLabel
        {
            get
            {
                var summaryInfo = _unfolded ? string.Empty: $"\n{Summary}";
                return $"<size=12><b>{Title}</b></size><i>{summaryInfo}</i>";
            }
        }


        /// <summary>draw title bar of current node</summary>
        protected virtual void DrawTitleBar()
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("<b>" + (_unfolded ? "▼ " : "► ") + "</b>" + TitleBarLabel, Styles.leftLabel);

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
            if (e.type == EventType.MouseDown && e.button == 0 && titleRect.Contains(e.mousePosition))
            {
                _unfolded = !_unfolded;
                e.Use();
            }
        }

        /// <summary>reset current node's parameters</summary>
        public virtual void Reset() => Utils.ResetObject(this);

        protected virtual void OnInspectorGUI(){}
        protected virtual GenericMenu GetContextMenu() => 
            new GenericMenu();
#endif
    }
}