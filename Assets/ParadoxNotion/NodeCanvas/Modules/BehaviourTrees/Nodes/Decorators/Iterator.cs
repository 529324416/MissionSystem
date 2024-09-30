using System.Collections;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using ParadoxNotion;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Iterate")]
    [Category("Decorators")]
    [Description("Iterates a list and executes its child once for each element in that list. Keeps iterating until the Termination Policy is met or until the whole list is iterated, in which case the last iteration child status is returned.")]
    [ParadoxNotion.Design.Icon("List")]
    public class Iterator : BTDecorator
    {

        public enum TerminationConditions
        {
            None,
            FirstSuccess,
            FirstFailure
        }

        [RequiredField]
        [BlackboardOnly]
        [Tooltip("The list to iterate.")]
        public BBParameter<IList> targetList;

        [BlackboardOnly]
        [Name("Current Element")]
        [Tooltip("Store the currently iterated list element in a variable.")]
        public BBObjectParameter current;

        [BlackboardOnly]
        [Name("Current Index")]
        [Tooltip("Store the currently iterated list index in a variable.")]
        public BBParameter<int> storeIndex;

        [Name("Termination Policy"), Tooltip("The condition for when to terminate the iteration and return status.")]
        public TerminationConditions terminationCondition = TerminationConditions.None;

        [Tooltip("The maximum allowed iterations. Leave at -1 to iterate the whole list.")]
        public BBParameter<int> maxIteration = -1;

        [Tooltip("Should the iteration start from the begining after the Iterator node resets?")]
        public bool resetIndex = true;

        private int currentIndex;

        private IList list => targetList != null ? targetList.value : null;

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( decoratedConnection == null ) {
                return Status.Optional;
            }

            if ( list == null || list.Count == 0 ) {
                return Status.Failure;
            }

            for ( var i = currentIndex; i < list.Count; i++ ) {

                current.value = list[i];
                storeIndex.value = i;
                status = decoratedConnection.Execute(agent, blackboard);

                if ( status == Status.Success && terminationCondition == TerminationConditions.FirstSuccess ) {
                    return Status.Success;
                }

                if ( status == Status.Failure && terminationCondition == TerminationConditions.FirstFailure ) {
                    return Status.Failure;
                }

                if ( status == Status.Running ) {
                    currentIndex = i;
                    return Status.Running;
                }


                if ( currentIndex == list.Count - 1 || currentIndex == maxIteration.value - 1 ) {
                    if ( resetIndex ) { currentIndex = 0; }
                    return status;
                }

                decoratedConnection.Reset();
                currentIndex++;
            }

            return Status.Running;
        }


        protected override void OnReset() {
            if ( resetIndex ) { currentIndex = 0; }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {

            GUILayout.Label("For Each\t" + current + "\nIn\t" + targetList, Styles.leftLabel);
            if ( terminationCondition != TerminationConditions.None ) {
                GUILayout.Label("Break on " + terminationCondition.ToString());
            }

            if ( Application.isPlaying ) {
                GUILayout.Label("Index: " + currentIndex.ToString() + " / " + ( list != null && list.Count != 0 ? ( list.Count - 1 ).ToString() : "?" ));
            }
        }

        protected override void OnNodeInspectorGUI() {
            DrawDefaultInspector();
            var argType = targetList.refType != null ? targetList.refType.GetEnumerableElementType() : null;
            if ( current.varType != argType ) { current.SetType(argType); }
        }
#endif

    }
}