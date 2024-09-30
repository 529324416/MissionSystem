using System.Collections.Generic;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{

    [Category("Composites")]
    [Description("Executes one child based on the provided int or enum case and returns its status.")]
    [ParadoxNotion.Design.Icon("IndexSwitcher")]
    [Color("b3ff7f")]
    public class Switch : BTComposite
    {

        public enum CaseSelectionMode
        {
            IndexBased = 0,
            EnumBased = 1
        }

        public enum OutOfRangeMode
        {
            ReturnFailure,
            LoopIndex
        }

        [Tooltip("If true and the 'case' change while a child is running, that child will immediately be interrupted and the new child will be executed.")]
        public bool dynamic;
        [Tooltip("The selection mode used.")]
        public CaseSelectionMode selectionMode = CaseSelectionMode.IndexBased;

        [ShowIf("selectionMode", 0)]
        public BBParameter<int> intCase;
        [ShowIf("selectionMode", 0)]
        public OutOfRangeMode outOfRangeMode = OutOfRangeMode.LoopIndex;

        [ShowIf("selectionMode", 1), BlackboardOnly]
        public BBObjectParameter enumCase = new BBObjectParameter(typeof(System.Enum));
        private Dictionary<int, int> enumCasePairing;

        private int current;
        private int runningIndex;

        public override void OnGraphStarted() {
            if ( selectionMode == CaseSelectionMode.EnumBased ) {
                var enumValue = enumCase.value;
                if ( enumValue != null ) {
                    enumCasePairing = new Dictionary<int, int>();
                    var enumValues = System.Enum.GetValues(enumValue.GetType());
                    for ( var i = 0; i < enumValues.Length; i++ ) {
                        enumCasePairing[(int)enumValues.GetValue(i)] = i;
                    }
                }
            }
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( outConnections.Count == 0 ) {
                return Status.Optional;
            }

            if ( status == Status.Resting || dynamic ) {

                if ( selectionMode == CaseSelectionMode.IndexBased ) {
                    current = intCase.value;
                    if ( outOfRangeMode == OutOfRangeMode.LoopIndex ) {
                        current = Mathf.Abs(current) % outConnections.Count;
                    }

                } else {
                    current = enumCasePairing[(int)enumCase.value];
                }

                if ( runningIndex != current ) {
                    outConnections[runningIndex].Reset();
                }

                if ( current < 0 || current >= outConnections.Count ) {
                    return Status.Failure;
                }
            }

            status = outConnections[current].Execute(agent, blackboard);

            if ( status == Status.Running ) {
                runningIndex = current;
            }

            return status;
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        public override string GetConnectionInfo(int i) {
            if ( selectionMode == CaseSelectionMode.EnumBased ) {
                if ( enumCase.value == null ) {
                    return "Null Enum".FormatError();
                }
                var enumNames = System.Enum.GetNames(enumCase.value.GetType());
                if ( i >= enumNames.Length ) {
                    return "Never".FormatError();
                }
                return enumNames[i];
            }
            return i.ToString();
        }

        protected override void OnNodeGUI() {
            if ( dynamic ) {
                GUILayout.Label("<b>DYNAMIC</b>");
            }
            GUILayout.Label(selectionMode == CaseSelectionMode.IndexBased ? ( "Current = " + intCase.ToString() ) : enumCase.ToString());
        }

        protected override void OnNodeInspectorGUI() {
            base.OnNodeInspectorGUI();
            if ( selectionMode == CaseSelectionMode.EnumBased ) {
                if ( enumCase.value != null ) {
                    GUILayout.BeginVertical("box");
                    foreach ( var s in System.Enum.GetNames(enumCase.value.GetType()) ) {
                        GUILayout.Label(s);
                    }
                    GUILayout.EndVertical();
                }
            }
        }

#endif
    }
}