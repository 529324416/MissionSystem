using ParadoxNotion.Design;
using UnityEngine;


namespace RedSaw.MissionSystem
{
    [Name("Dice"), Description("Roll a dice with given probability")]
    public class Dice : ConditionBase
    {
        [SerializeField] public float probability = 0.5f;
        public override bool IsConditionMet => Random.value < probability;

#if UNITY_EDITOR
        public override string Summary => probability.ToString("P1");

        protected override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            probability = UnityEditor.EditorGUILayout.Slider("Probability", probability, 0, 1);
        }
#endif
    }
}