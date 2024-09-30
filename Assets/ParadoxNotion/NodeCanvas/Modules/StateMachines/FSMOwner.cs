using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines
{

    ///<summary> Add this component on a gameobject to behave based on an FSM.</summary>
    [AddComponentMenu("NodeCanvas/FSM Owner")]
    public class FSMOwner : GraphOwner<FSM>
    {

        ///<summary>The current state name of the root fsm.</summary>
        public string currentRootStateName => behaviour?.currentStateName;

        ///<summary>The previous state name of the root fsm.</summary>
        public string previousRootStateName => behaviour?.previousStateName;

        ///<summary>The current deep state name of the fsm including sub fsms if any.</summary>
        public string currentDeepStateName => GetCurrentState(true)?.name;

        ///<summary>The previous deep state name of the fsm including sub fsms if any.</summary>
        public string previousDeepStateName => GetPreviousState(true)?.name;

        ///<summary>Returns the current fsm state optionally recursively by including SubFSMs.</summary>
        public IState GetCurrentState(bool includeSubFSMs = true) {
            if ( behaviour == null ) { return null; }
            var current = behaviour.currentState;
            if ( includeSubFSMs ) {
                while ( current is NestedFSMState subState ) {
                    current = subState.currentInstance?.currentState;
                }
            }
            return current;
        }

        ///<summary>Returns the previous fsm state optionally recursively by including SubFSMs.</summary>
        public IState GetPreviousState(bool includeSubFSMs = true) {
            if ( behaviour == null ) { return null; }
            var current = behaviour.currentState;
            var previous = behaviour.previousState;
            if ( includeSubFSMs ) {
                while ( current is NestedFSMState subState ) {
                    current = subState.currentInstance?.currentState;
                    previous = subState.currentInstance?.previousState;
                }
            }
            return previous;
        }


        ///<summary>Enter a state of the root FSM by its name.</summary>
        public IState TriggerState(string stateName) { return TriggerState(stateName, FSM.TransitionCallMode.Normal); }
        public IState TriggerState(string stateName, FSM.TransitionCallMode callMode) {
            return behaviour?.TriggerState(stateName, callMode);
        }

        ///<summary>Get all root state names, excluding non-named states.</summary>
        public string[] GetStateNames() {
            return behaviour?.GetStateNames();
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmos() {
            UnityEditor.Handles.Label(transform.position + new Vector3(0, -0.3f, 0), currentDeepStateName, Styles.centerLabel);
        }
#endif
    }
}