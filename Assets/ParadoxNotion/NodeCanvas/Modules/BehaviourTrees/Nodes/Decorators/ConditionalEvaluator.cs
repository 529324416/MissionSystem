using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Conditional")]
    [Category("Decorators")]
    [Description("Executes and returns the child status only if the condition is true. Returns Failure if the condition is false.")]
    [ParadoxNotion.Design.Icon("Accessor")]
    public class ConditionalEvaluator : BTDecorator, ITaskAssignable<ConditionTask>
    {

        [Name("Dynamic"), Tooltip("If enabled, the condition is re-evaluated per frame and the child is aborted if the condition becomes false.")]
        public bool isDynamic;
        [Tooltip("The status that will be returned if the assigned condition is or becomes false.")]
        public CompactStatus conditionFailReturn = CompactStatus.Failure;

        [SerializeField]
        private ConditionTask _condition;
        private bool accessed;

        public Task task {
            get { return condition; }
            set { condition = (ConditionTask)value; }
        }

        private ConditionTask condition {
            get { return _condition; }
            set { _condition = value; }
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            if ( condition == null ) {
                return decoratedConnection.Execute(agent, blackboard);
            }

            if ( status == Status.Resting ) {
                condition.Enable(agent, blackboard);
            }

            if ( isDynamic ) {

                if ( condition.Check(agent, blackboard) ) {
                    return decoratedConnection.Execute(agent, blackboard);
                }
                decoratedConnection.Reset();
                return (Status)conditionFailReturn;

            } else {

                if ( status != Status.Running ) {
                    accessed = condition.Check(agent, blackboard);
                }

                return accessed ? decoratedConnection.Execute(agent, blackboard) : (Status)conditionFailReturn;
            }
        }

        protected override void OnReset() {
            if ( condition != null ) { condition.Disable(); }
            accessed = false;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            if ( isDynamic ) { GUILayout.Label("<b>DYNAMIC</b>"); }
        }

        protected override void OnNodeInspectorGUI() {
            base.OnNodeInspectorGUI();
            EditorUtils.Separator();
        }

#endif
    }
}