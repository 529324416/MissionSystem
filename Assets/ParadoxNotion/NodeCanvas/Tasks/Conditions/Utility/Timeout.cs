using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Utility")]
    [Description("Will return true after a specific amount of time has passed and false while still counting down")]
    public class Timeout : ConditionTask
    {
        public BBParameter<float> timeout = 1f;
        private float startTime;
        private float elapsedTime => ownerSystem.elapsedTime - startTime;

        protected override string info {
            get { return string.Format("Timeout {0}/{1}", elapsedTime.ToString("0.00"), timeout.ToString()); }
        }

        protected override void OnEnable() {
            startTime = ownerSystem.elapsedTime;
        }

        protected override bool OnCheck() {
            return elapsedTime >= timeout.value;
        }
    }
}
