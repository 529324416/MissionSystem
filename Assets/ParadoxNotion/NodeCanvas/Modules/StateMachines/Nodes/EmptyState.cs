using ParadoxNotion.Design;

namespace NodeCanvas.StateMachines
{

    [Description("This node has no functionality and you can use this for organization.\nIn comparison to an empty Action State, Transitions here are immediately evaluated in the same frame that this node is entered.")]
    [Color("6ebbff")]
    [Name("Pass", 98)]
    public class EmptyState : FSMState
    {

        public override string name {
            get { return base.name.ToUpper(); }
        }

        protected override void OnEnter() {
            Finish();
            CheckTransitions();
        }
    }
}