using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Utility")]
    [Description("Return true or false based on the probability settings. The chance is rolled for once whenever the condition is enabled.")]
    public class Probability : ConditionTask
    {

        public BBParameter<float> probability = 0.5f;
        public BBParameter<float> maxValue = 1;

        private bool success;

        protected override string info {
            get { return ( probability.value / maxValue.value * 100 ) + "%"; }
        }

        protected override void OnEnable() {
            success = Random.Range(0f, maxValue.value) <= probability.value;
        }

        protected override bool OnCheck() {
            return success;
        }
    }
}