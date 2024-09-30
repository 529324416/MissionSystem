using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Optional")]
    [Category("Decorators")]
    [Description("Executes the decorated child as normal and returns an Optional status, thus making it optional to the parent node in regards to what status is returned.\nThis has the same effect as disabling the node, but instead it executes normaly.")]
    [ParadoxNotion.Design.Icon("UpwardsArrow")]
    public class Optional : BTDecorator
    {

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            if ( status == Status.Resting ) {
                decoratedConnection.Reset();
            }

            status = decoratedConnection.Execute(agent, blackboard);
            return status == Status.Running ? Status.Running : Status.Optional;
        }
    }
}