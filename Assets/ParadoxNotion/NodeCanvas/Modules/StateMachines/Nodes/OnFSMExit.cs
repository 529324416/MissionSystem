using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines
{

    [Description("Execute a number of Actions when the FSM stops/exits, if Conditions are met. Note that the actions will only execute for 1 frame. This is not a state.")]
    [Color("ff64cb")]
    [ParadoxNotion.Design.Icon("MacroOut")]
    [Name("On FSM Exit")]
    public class OnFSMExit : FSMNode
    {

        [SerializeField] private ConditionList _conditionList;
        [SerializeField] private ActionList _actionList;

        public override string name => base.name.ToUpper();
        public override int maxInConnections => 0;
        public override int maxOutConnections => 0;
        public override bool allowAsPrime => false;

        ///----------------------------------------------------------------------------------------------

        public override void OnValidate(Graph assignedGraph) {
            if ( _conditionList == null ) {
                _conditionList = (ConditionList)Task.Create(typeof(ConditionList), assignedGraph);
                _conditionList.checkMode = ConditionList.ConditionsCheckMode.AllTrueRequired;
            }
            if ( _actionList == null ) {
                _actionList = (ActionList)Task.Create(typeof(ActionList), assignedGraph);
                _actionList.executionMode = ActionList.ActionsExecutionMode.ActionsRunInParallel;
            }
        }

        public override void OnGraphStarted() {
            _conditionList.Enable(graphAgent, graphBlackboard);
        }

        public override void OnGraphStoped() {
            if ( _conditionList.Check(graphAgent, graphBlackboard) ) {
                _actionList.Execute(graphAgent, graphBlackboard);
            }
            _actionList.EndAction(null);
            _conditionList.Disable();
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            if ( _conditionList.conditions.Count > 0 ) {
                GUILayout.BeginVertical(Styles.roundedBox);
                GUILayout.Label(_conditionList.summaryInfo);
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical(Styles.roundedBox);
            GUILayout.Label(_actionList.summaryInfo);
            GUILayout.EndVertical();

            base.OnNodeGUI();
        }

        protected override void OnNodeInspectorGUI() {
            EditorUtils.CoolLabel("Conditions (optional)");
            _conditionList.ShowListGUI();
            _conditionList.ShowNestedConditionsGUI();
            EditorUtils.BoldSeparator();
            EditorUtils.CoolLabel("Actions");
            _actionList.ShowListGUI();
            _actionList.ShowNestedActionsGUI();
        }

#endif
    }
}