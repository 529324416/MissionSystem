using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Action")]
    [Description("Executes an action and returns Success or Failure when the action is finished.\nReturns Running until the action is finished.")]
    [ParadoxNotion.Design.Icon("Action")]
    // [Color("ff6d53")]
    public class ActionNode : BTNode, ITaskAssignable<ActionTask>
    {

        [SerializeField]
        private ActionTask _action;

        public Task task {
            get { return action; }
            set { action = (ActionTask)value; }
        }

        public ActionTask action {
            get { return _action; }
            set { _action = value; }
        }

        public override string name {
            get { return base.name.ToUpper(); }
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( action == null ) {
                return Status.Optional;
            }

            if ( status == Status.Resting || status == Status.Running ) {
                return action.Execute(agent, blackboard);
            }

            return status;
        }

        protected override void OnReset() {
            if ( action != null ) {
                action.EndAction(null);
            }
        }

        public override void OnGraphPaused() {
            if ( action != null ) {
                action.Pause();
            }
        }
    }
}