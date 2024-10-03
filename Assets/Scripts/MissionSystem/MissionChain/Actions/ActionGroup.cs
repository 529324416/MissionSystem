using System.Text;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace RedSaw.MissionSystem
{
    [DoNotList]
    public class ActionGroup : ActionBase
    {
        [SerializeField] private readonly List<ActionBase> _actions = new List<ActionBase>();
        public override void Execute()
        {
            foreach (var action in _actions)
                action.Execute();
        }
        
        public int Count => _actions.Count;
        public IEnumerable<ActionBase> allActions => _actions;
        
        public ActionBase First
        {
            get
            {
                if (_actions.Count > 0) return _actions[0];
                return null;
            }
        }

        /// <summary>
        /// add a new action to this group
        /// </summary>
        /// <param name="action"></param>
        public void AddAction(ActionBase action)
        {
            switch (action)
            {
                case null:
                    return;
                case ActionGroup group:
                {
                    foreach (var act in group.allActions)
                        AddAction(act);
                    return;
                }
                default:
                    _actions.Add(action);
                    break;
            }
        }

        /// <summary>remove action from current group</summary>
        /// <param name="action"></param>
        public void RemoveAction(ActionBase action)
        {
            switch (action)
            {
                case null:
                    return;
                case ActionGroup group:
                {
                    foreach (var act in group.allActions)
                        RemoveAction(act);
                    return;
                }
                default:
                    _actions.Remove(action);
                    break;
            }
        }

#if UNITY_EDITOR
        private int selectedIdx = 0;
        public override string Title => "Action Group";

        public override string Summary
        {
            get
            {
                var result = new StringBuilder();
                foreach (var action in _actions)
                    result.Append(action.Summary + "\n");
                return result.ToString().Trim('\n');
            }
        }

        public sealed override void Reset()
        {
            foreach (var action in _actions)
                action.Reset();
        }

        protected override void OnInspectorGUI()
        {
            EditorUtils.ReorderableList(_actions, (idx, selected) =>
            {
                var _action = _actions[idx];

                /* 
                    绘制动作摘要信息和删除按钮
                    draw action summaryInfo and delete button 
                */
                GUI.color = Color.white.WithAlpha(selectedIdx == idx ? 0.75f : 0.25f);
                GUILayout.BeginHorizontal("box");
                GUI.color = Color.white.WithAlpha(0.8f);
                GUILayout.Label($"<size=13>{_action.Summary}</size>");
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    UndoUtility.RecordObject(_node.graph, "Action Removed");
                    _node.DeleteAction(_action);
                }
                GUILayout.EndHorizontal();

                /* 
                    点击时选中目标选项
                    selected target option while clicked 
                */
                var lastRect = GUILayoutUtility.GetLastRect();
                EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.Link);
                var e = Event.current;
                if (e.type == EventType.MouseDown && e.button == 0 && lastRect.Contains(e.mousePosition))
                {
                    selectedIdx = idx;
                    _actions[selectedIdx]._unfolded = true;
                }

                GUI.color = Color.white;
            });
                
            /* 
                绘制选中的行为节点的检查器
                draw selected action's inspector
            */                
            if (selectedIdx >= 0)
            {
                var selectedAction = _actions[selectedIdx];
                selectedAction.DrawInspector();
            }
        }
#endif
    }
}