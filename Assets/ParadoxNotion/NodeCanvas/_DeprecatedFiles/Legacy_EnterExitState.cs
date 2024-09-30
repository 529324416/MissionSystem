using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines
{

    [Name("Enter | Exit")]
    [Description("Execute a number of Actions when the FSM enters/starts and when it exits/stops. This is not a state.")]
    [Color("ff64cb")]
    [ParadoxNotion.Design.Icon("MacroIn")]
    [System.Obsolete("Use On FSM Enter and On FSM Exit nodes")]
    public class EnterExitState : FSMNode, IUpdatable
    {

        [SerializeField] private ActionList _actionListEnter;
        [SerializeField] private ActionList _actionListExit;

        public ActionList actionListEnter {
            get { return _actionListEnter; }
            set { _actionListEnter = value; }
        }

        public ActionList actionListExit {
            get { return _actionListExit; }
            set { _actionListExit = value; }
        }

        public override string name => base.name.ToUpper();
        public override int maxInConnections => 0;
        public override int maxOutConnections => 0;
        public override bool allowAsPrime => false;

        ///----------------------------------------------------------------------------------------------

        public override void OnValidate(Graph assignedGraph) {
            if ( actionListEnter == null ) {
                actionListEnter = (ActionList)Task.Create(typeof(ActionList), assignedGraph);
                actionListEnter.executionMode = ActionList.ActionsExecutionMode.ActionsRunInParallel;
            }
            if ( actionListExit == null ) {
                actionListExit = (ActionList)Task.Create(typeof(ActionList), assignedGraph);
                actionListExit.executionMode = ActionList.ActionsExecutionMode.ActionsRunInParallel;
            }
        }

        public override void OnGraphStarted() {
            status = actionListEnter.Execute(graphAgent, graphBlackboard);
        }

        public override void OnGraphStoped() {
            actionListExit.Execute(graphAgent, graphBlackboard);
            actionListExit.EndAction(null);
            actionListEnter.EndAction(null);
        }

        void IUpdatable.Update() {
            if ( status == Status.Running ) {
                status = actionListEnter.Execute(graphAgent, graphBlackboard);
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.BeginVertical(Styles.roundedBox);
            GUILayout.Label(actionListEnter.summaryInfo);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(Styles.roundedBox);
            GUILayout.Label(actionListExit.summaryInfo);
            GUILayout.EndVertical();
            base.OnNodeGUI();
        }

        protected override void OnNodeInspectorGUI() {
            EditorUtils.CoolLabel("FSM Enter Actions");
            actionListEnter.ShowListGUI();
            actionListEnter.ShowNestedActionsGUI();
            EditorUtils.BoldSeparator();
            EditorUtils.CoolLabel("FSM Exit Actions");
            actionListExit.ShowListGUI();
            actionListExit.ShowNestedActionsGUI();
        }

#endif
    }
}