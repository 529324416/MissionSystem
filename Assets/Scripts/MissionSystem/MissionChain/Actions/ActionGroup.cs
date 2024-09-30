using System.Collections.Generic;
using System.Text;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace RedSaw.MissionSystem
{
    [DoNotList]
    public class ActionGroup : ActionBase
    {
        private readonly List<ActionBase> _actions = new List<ActionBase>();
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
        private int selectedIdx = -1;
        public override string Title => "Action Group";

        public override string Summary
        {
            get
            {
                var result = new StringBuilder();
                foreach (var action in _actions)
                    result.Append(action.Summary + "\n");
                return result.ToString();
            }
        }
        
        public override void DrawInspector()
        {
            DrawTitleBar();
            if (_unfolded)
            {
                EditorUtils.ReorderableList(_actions, (idx, selected) =>
                {
                    var _action = _actions[idx];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(_action.Summary);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        UndoUtility.RecordObject(_node.graph, "Action Removed");
                        _node.DeleteAction(_action);
                    }
                    GUILayout.EndHorizontal();
                    if (selected) selectedIdx = idx;
                });
                
                if (selectedIdx != -1)
                {
                    var selectedAction = _actions[selectedIdx];
                    selectedAction.DrawInspector();
                }
            }
        }
#endif
    }
}