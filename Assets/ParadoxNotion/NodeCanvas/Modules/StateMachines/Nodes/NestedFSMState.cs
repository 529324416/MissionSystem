using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines
{


    [Name("Sub FSM")]
    [Description("Execute a sub FSM OnEnter, and Stop that FSM OnExit. This state is Finished only when and if the sub FSM is finished as well.")]
    [DropReferenceType(typeof(FSM))]
    [ParadoxNotion.Design.Icon("FSM")]
    public class NestedFSMState : FSMStateNested<FSM>
    {

        public enum FSMExitMode
        {
            StopAndRestart,
            PauseAndResume
        }

        [SerializeField, ExposeField, Name("Sub FSM")]
        private BBParameter<FSM> _nestedFSM = null;

        [Tooltip("What will happen to the sub FSM when this state exits.")]
        public FSMExitMode exitMode;

        public override FSM subGraph { get { return _nestedFSM.value; } set { _nestedFSM.value = value; } }
        public override BBParameter subGraphParameter => _nestedFSM;

        //

        protected override void OnEnter() {
            if ( subGraph == null ) {
                Finish(false);
                return;
            }

            this.TryStartSubGraph(graphAgent, Finish);
            OnUpdate();
        }

        protected override void OnUpdate() {
            currentInstance.UpdateGraph(this.graph.deltaTime);
        }

        protected override void OnExit() {
            if ( currentInstance != null ) {
                if ( this.status == Status.Running ) {
                    this.TryReadAndUnbindMappedVariables();
                }
                if ( exitMode == FSMExitMode.StopAndRestart ) {
                    currentInstance.Stop();
                } else {
                    currentInstance.Pause();
                }
            }
        }
    }
}