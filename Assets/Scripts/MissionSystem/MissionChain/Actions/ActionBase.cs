using UnityEngine;
using ParadoxNotion.Design;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RedSaw.MissionSystem
{
    /// <summary>base class of mission actions</summary>
    public abstract class ActionBase : MissionChainObject
    {

        /// <summary>perform action with current parameters</summary>
        public abstract void Execute();

#if UNITY_EDITOR
        public NodeAction _node;

        protected override GenericMenu GetContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Open Script"), false, () => EditorUtils.OpenScriptOfType(this.GetType()));
            menu.AddItem(new GUIContent("Copy"), false, () => { CopyBuffer.SetCache(this); });
            if (CopyBuffer.TryGetCache<ActionBase>(out var copiedAction) &&
                this.GetType().IsInstanceOfType(copiedAction))
                menu.AddItem(new GUIContent("Paste"), false, () =>
                {
                    UndoUtility.RecordObject(_node.graph, "Action Pasted");
                    Utils.CopyObjectFrom(this, copiedAction);
                });
            menu.AddItem(new GUIContent("Reset"), false, () =>
            {
                UndoUtility.RecordObject(_node.graph, "Action Reset");
                Reset();
            });
            menu.AddSeparator("/");
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                UndoUtility.RecordObject(_node.graph, "Action Removed");
                _node.DeleteAction(this);
            });
            menu.AddSeparator("/");
            return OnCreateContextMenu(menu);
        }

        protected virtual GenericMenu OnCreateContextMenu(GenericMenu menu) => menu;
#endif
    }
}