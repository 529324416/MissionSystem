using System;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace RedSaw.MissionSystem
{
    public abstract class NodeBase : Node
    {
        public override Alignment2x2 iconAlignment => Alignment2x2.Bottom;
        public override Alignment2x2 commentsAlignment => Alignment2x2.Bottom;
        public override bool canSelfConnect => false;
        public override int maxInConnections => -1;
        public override int maxOutConnections => -1;
        public override bool allowAsPrime => false;
        public override Type outConnectionType => typeof(ConnectionBase);
    }
}