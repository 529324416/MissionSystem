using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Selector", 9)]
    [Category("Composites")]
    [Description("Executes its childrfen in order and returns Failure if all children return Failure. As soon as a child returns Success, the Selector will stop and return Success as well.")]
    [ParadoxNotion.Design.Icon("Selector")]
    [Color("b3ff7f")]
    public class Selector : BTComposite
    {

        [Tooltip("If true, then higher priority children are re-evaluated per frame and if either returns Success, then the Selector will immediately stop and return Success as well.")]
        public bool dynamic;
        [Tooltip("If true, the children order of execution is shuffled each time the Selector resets.")]
        public bool random;

        private int lastRunningNodeIndex;

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            for ( var i = dynamic ? 0 : lastRunningNodeIndex; i < outConnections.Count; i++ ) {

                status = outConnections[i].Execute(agent, blackboard);

                switch ( status ) {
                    case Status.Running:

                        if ( dynamic && i < lastRunningNodeIndex ) {
                            for ( var j = i + 1; j <= lastRunningNodeIndex; j++ ) {
                                outConnections[j].Reset();
                            }
                        }

                        lastRunningNodeIndex = i;
                        return Status.Running;

                    case Status.Success:

                        if ( dynamic && i < lastRunningNodeIndex ) {
                            for ( var j = i + 1; j <= lastRunningNodeIndex; j++ ) {
                                outConnections[j].Reset();
                            }
                        }

                        return Status.Success;
                }
            }

            return Status.Failure;
        }

        protected override void OnReset() {
            lastRunningNodeIndex = 0;
            if ( random ) { outConnections = outConnections.Shuffle(); }
        }

        public override void OnChildDisconnected(int index) {
            if ( index != 0 && index == lastRunningNodeIndex ) {
                lastRunningNodeIndex--;
            }
        }

        public override void OnGraphStarted() { OnReset(); }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------

#if UNITY_EDITOR
        protected override void OnNodeGUI() {
            if ( dynamic ) { GUILayout.Label("<b>DYNAMIC</b>"); }
            if ( random ) { GUILayout.Label("<b>RANDOM</b>"); }
        }
#endif

    }
}