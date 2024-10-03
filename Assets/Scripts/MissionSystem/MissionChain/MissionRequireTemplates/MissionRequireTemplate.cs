using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace RedSaw.MissionSystem
{
    /// <summary>require template base type</summary>
    public abstract class MissionRequireTemplate : MissionRequire<object>
    {
        public abstract class MissionRequireTemplateHandle : MissionRequireHandle<object>
        {
            protected MissionRequireTemplateHandle(MissionRequireTemplate require) : base(require) { }
        }
        

#if UNITY_EDITOR
        public bool _unfolded;
        private string _title;
        private string _summary;
        private string _description;
        public NodeMission _node;
        
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

        public void DrawInspectorGUI()
        {
            GUILayout.BeginVertical();
            DrawTitleBar();
            if (_unfolded)
            {
                EditorGUILayout.HelpBox(Description, MessageType.None);
                OnInspectorGUI();
            }
            GUILayout.EndVertical();
        }

        protected void DrawTitleBar()
        {
            GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.black.WithAlpha(0.3f) : Color.white.WithAlpha(0.5f);
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                string _summaryInfo = $"<size=12><color=#a9a9a9>{Summary}</color></size>";
                GUILayout.Label(
                    "<b>" + (_unfolded ? "▼ " : "► ") + _summaryInfo + "</b>"
                    , Styles.leftLabel);

                /* C# icon, open script while clicked */
                if (GUILayout.Button(Icons.csIcon, GUI.skin.label, GUILayout.Width(20), GUILayout.Height(20)))
                    EditorUtils.OpenScriptOfType(this.GetType());

                /* gear icon, open context menu while clicked */
                if (GUILayout.Button(Icons.gearPopupIcon, Styles.centerLabel, GUILayout.Width(20), GUILayout.Height(20)))
                    GetContextMenu().ShowAsContext();
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            /* unfold control */
            var titleRect = GUILayoutUtility.GetLastRect();
            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && titleRect.Contains(e.mousePosition))
            {
                _unfolded = !_unfolded;
                e.Use();
            }
        }

        protected GenericMenu GetContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy"), false, () => CopyBuffer.SetCache(this));
            menu.AddItem(new GUIContent("Reset"), false, Reset);
            if(CopyBuffer.TryGetCache<MissionRequireTemplate>(out var cache) && cache != this && cache.GetType() == GetType())
                menu.AddItem(new GUIContent("Paste"), false, () => Utils.CopyObjectFrom(this, cache));
            else
                menu.AddDisabledItem(new GUIContent("Paste"));

            menu.AddSeparator("/");
            menu.AddItem(new GUIContent("Delete"), false, () => _node.DeleteRequire(this));
            menu = OnCreateContextMenu(menu);
            return menu;
        }

        /// <summary>overwrite this function if you try to 
        /// manually reset require template</summary>
        protected virtual void Reset()
        {
            UndoUtility.RecordObject(_node.graph, "Require Reset");
            Utils.ResetObject(this);
        }

        /// <summary>
        /// overwrite this function if you try to 
        /// add more options into context menu
        /// </summary>
        protected virtual GenericMenu OnCreateContextMenu(GenericMenu menu) => menu;

        /// <summary> draw inspector gui </summary>
        protected virtual void OnInspectorGUI() {}
#endif
    }
}