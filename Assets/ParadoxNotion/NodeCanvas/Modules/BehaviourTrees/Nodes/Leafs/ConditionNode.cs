using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Condition")]
    [Description("Checks a condition and returns Success or Failure.")]
    [ParadoxNotion.Design.Icon("Condition")]
    // [Color("ff6d53")]
    public class ConditionNode : BTNode, ITaskAssignable<ConditionTask>
    {

        [SerializeField]
        private ConditionTask _condition;

        public Task task {
            get { return condition; }
            set { condition = (ConditionTask)value; }
        }

        public ConditionTask condition {
            get { return _condition; }
            set { _condition = value; }
        }

        public override string name {
            get { return base.name.ToUpper(); }
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {
            if ( condition == null ) {
                return Status.Optional;
            }

            if ( status == Status.Resting ) {
                condition.Enable(agent, blackboard);
            }

            return condition.Check(agent, blackboard) ? Status.Success : Status.Failure;
        }

        protected override void OnReset() {
            if ( condition != null ) { condition.Disable(); }
        }
    }
}