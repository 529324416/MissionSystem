using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
using NavMesh = UnityEngine.AI.NavMesh;
using NavMeshHit = UnityEngine.AI.NavMeshHit;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Movement/Pathfinding")]
    [Description("Makes the agent wander randomly within the navigation map")]
    public class Wander : ActionTask<NavMeshAgent>
    {

        [Tooltip("The speed to wander with.")]
        public BBParameter<float> speed = 4;
        [Tooltip("The distance to keep from each wander point.")]
        public BBParameter<float> keepDistance = 0.1f;
        [Tooltip("A wander point can't be closer than this distance")]
        public BBParameter<float> minWanderDistance = 5;
        [Tooltip("A wander point can't be further than this distance")]
        public BBParameter<float> maxWanderDistance = 20;
        [Tooltip("If enabled, will keep wandering forever. If not, only one wander point will be performed.")]
        public bool repeat = true;

        protected override void OnExecute() {
            agent.speed = speed.value;
            DoWander();
        }

        protected override void OnUpdate() {
            if ( !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance.value ) {
                if ( repeat ) {
                    DoWander();
                } else {
                    EndAction();
                }
            }
        }

        void DoWander() {
            var min = minWanderDistance.value;
            var max = maxWanderDistance.value;
            min = Mathf.Clamp(min, 0.01f, max);
            max = Mathf.Clamp(max, min, max);
            var wanderPos = agent.transform.position;
            while ( ( wanderPos - agent.transform.position ).magnitude < min ) {
                wanderPos = ( Random.insideUnitSphere * max ) + agent.transform.position;
            }

            NavMeshHit hit;
            if ( NavMesh.SamplePosition(wanderPos, out hit, agent.height * 2, NavMesh.AllAreas) ) {
                agent.SetDestination(hit.position);
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