using NodeCanvas.Editor;
using ParadoxNotion.Design;

namespace RedSaw.MissionSystem
{
    [ParadoxNotion.Design.Icon("Action"), Color("b1d480"), Name("Mission")]
    [Description("setup a new mission")]
    public class NodeMission : NodeBase
    {
        public override bool allowAsPrime => true;

        private string test;

#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI()
        {
             
        }
#endif
    }
}