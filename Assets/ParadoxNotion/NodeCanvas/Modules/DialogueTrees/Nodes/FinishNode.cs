using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.DialogueTrees
{

    [Name("FINISH")]
    [Category("Control")]
    [Description("End the dialogue in Success or Failure.\nNote: A Dialogue will anyway End in Succcess if it has reached a node without child connections. Thus this node is mostly useful if you want to end a Dialogue in Failure.")]
    [ParadoxNotion.Design.Icon("Halt")]
    [Color("6ebbff")]
    public class FinishNode : DTNode
    {

        public CompactStatus finishState = CompactStatus.Success;

        public override int maxOutConnections { get { return 0; } }
        public override bool requireActorSelection { get { return false; } }

        protected override Status OnExecute(Component agent, IBlackboard bb) {
            status = (Status)finishState;
            DLGTree.Stop(finishState == CompactStatus.Success ? true : false);
            return status;
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label("<b>" + finishState.ToString() + "</b>");
        }

        protected override void OnNodeInspectorGUI() {
            DrawDefaultInspector();
        }

#endif
    }
}