﻿using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Movement/Pathfinding")]
    [Description("Flees away from the target")]
    public class Flee : ActionTask<NavMeshAgent>
    {

        [RequiredField, Tooltip("The target to flee from.")]
        public BBParameter<GameObject> target;
        [Tooltip("The speed to flee.")]
        public BBParameter<float> speed = 4f;
        [Tooltip("The distance to flee at.")]
        public BBParameter<float> fledDistance = 10f;
        [Tooltip("A distance to look away from the target for valid flee destination.")]
        public BBParameter<float> lookAhead = 2f;

        protected override string info {
            get { return string.Format("Flee from {0}", target); }
        }

        protected override void OnExecute() {
            if ( target.value == null ) { EndAction(false); return; }
            agent.speed = speed.value;
            if ( ( agent.transform.position - target.value.transform.position ).magnitude >= fledDistance.value ) {
                EndAction(true);
                return;
            }
        }

        protected override void OnUpdate() {
            if ( target.value == null ) { EndAction(false); return; }
            var targetPos = target.value.transform.position;
            if ( ( agent.transform.position - targetPos ).magnitude >= fledDistance.value ) {
                EndAction(true);
                return;
            }

            var fleePos = targetPos + ( agent.transform.position - targetPos ).normalized * ( fledDistance.value + lookAhead.value + agent.stoppingDistance );
            if ( !agent.SetDestination(fleePos) ) {
                EndAction(false);
            }
        }


        protected override void OnPause() { OnStop(); }
        protected override void OnStop() {
            if ( agent.gameObject.activeSelf ) {
                agent.ResetPath();
            }
        }
    }
}