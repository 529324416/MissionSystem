#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Framework
{

    partial class Graph
    {

        private int _childAssignableIndex = -1;

        ///<summary>EDITOR. Responsible for breacrumb navigation only</summary>
        public Graph GetCurrentChildGraph() {
            if ( _childAssignableIndex == -1 || _childAssignableIndex > allNodes.Count - 1 ) {
                return null;
            }
            var assignable = allNodes[_childAssignableIndex] as IGraphAssignable;
            if ( assignable != null ) {
                return assignable.subGraph;
            }
            return null;
        }

        ///<summary>EDITOR. Responsible for breacrumb navigation only</summary>
        public void SetCurrentChildGraphAssignable(IGraphAssignable assignable) {
            if ( assignable == null || assignable.subGraph == null ) {
                _childAssignableIndex = -1;
                return;
            }
            if ( Application.isPlaying && EditorUtility.IsPersistent(assignable.subGraph) ) {
                ParadoxNotion.Services.Logger.LogWarning("You can't view sub-graphs in play mode until they are initialized to avoid editing asset references accidentally", LogTag.EDITOR, this);
                _childAssignableIndex = -1;
                return;
            }
            assignable.subGraph.SetCurrentChildGraphAssignable(null);
            _childAssignableIndex = allNodes.IndexOf(assignable as Node);
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Editor. Returns a Generic Menu for on canvas click</summary>
        public GenericMenu CallbackOnCanvasContextMenu(GenericMenu menu, Vector2 canvasMousePos) { return OnCanvasContextMenu(menu, canvasMousePos); }
        ///<summary>Editor. Returns a Generic menu for on node click</summary>
        public GenericMenu CallbackOnNodesContextMenu(GenericMenu menu, Node[] nodes) { return OnNodesContextMenu(menu, nodes); }
        ///<summary>Editor. Invoke drag and drop on canvas for object</summary>
        public void CallbackOnDropAccepted(Object o, Vector2 canvasMousePos) {
            ///<summary>for all graphs, make possible to drag and drop IGraphAssignables</summary>
            foreach ( var type in Editor.GraphEditorUtility.GetDropedReferenceNodeTypes<IGraphAssignable>(o) ) {
                if ( baseNodeType.IsAssignableFrom(type) ) {
                    var node = (IGraphAssignable)AddNode(type, canvasMousePos);
                    node.subGraph = (Graph)o;
                    return;
                }
            }
            OnDropAccepted(o, canvasMousePos);
        }
        ///<summary>Editor. Invoke drag and drop on canvas for variable</summary>
        public void CallbackOnVariableDropInGraph(IBlackboard bb, Variable variable, Vector2 canvasMousePos) { OnVariableDropInGraph(bb, variable, canvasMousePos); }
        ///<summary>Editor. Allows adding more stuff in graph editor toolbar per graph instance</summary>
        public void CallbackOnGraphEditorToolbar() { OnGraphEditorToolbar(); }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Editor. Override to add extra context sensitive options in the right click canvas context menu</summary>
        virtual protected GenericMenu OnCanvasContextMenu(GenericMenu menu, Vector2 canvasMousePos) { return menu; }
        ///<summary>Editor. Override to add more entries to the right click context menu when multiple nodes are selected</summary>
        virtual protected GenericMenu OnNodesContextMenu(GenericMenu menu, Node[] nodes) { return menu; }
        ///<summary>Editor. Handle drag and drop objects in the graph</summary>
        virtual protected void OnDropAccepted(Object o, Vector2 canvasMousePos) { }
        ///<summary>Editor. Handle what happens when blackboard variable is drag and droped in graph</summary>
        virtual protected void OnVariableDropInGraph(IBlackboard bb, Variable variable, Vector2 canvasMousePos) { }
        ///<summary>Editor. Append stuff in graph editor toolbar</summary>
        virtual protected void OnGraphEditorToolbar() { }

        ///----------------------------------------------------------------------------------------------

    }
}

#endif
