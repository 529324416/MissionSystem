using System.Collections.Generic;
using System.Linq;

namespace RedSaw.MissionSystem
{
    public class MissionChainHandle
    {
        private readonly MissionChain chain;
        private readonly Dictionary<string, NodeMission> activeNodes = new Dictionary<string, NodeMission>();
        private readonly Queue<NodeMission> buffer = new Queue<NodeMission>();

        public bool IsCompleted => activeNodes.Count == 0;

        public MissionChainHandle(MissionChain chain)
        {
            this.chain = chain;
            
            /* execute prime node */
            if (chain.primeNode != null)
                ExecuteNode(chain.primeNode as NodeBase);
        }

        public void FlushBuffer(System.Action<MissionPrototype<object>> deployer)
        {
            if (buffer.Count == 0) return;
            while (buffer.Count > 0)
            {
                var node = buffer.Dequeue();
                var missionProto = node.MissionProto;
                activeNodes.Add(missionProto.id, node);
                deployer(missionProto);
            }
        }

        public void OnMissionComplete(string missionId, bool continues)
        {
            if (!activeNodes.Remove(missionId, out var node)) return;
            
            /* execute all available output connections */
            if (continues)
            {
                foreach (var outConnection in node.outConnections.Where(c => ((ConnectionBase)c).IsAvailable))
                    ExecuteNode(outConnection.targetNode as NodeBase);
            }
        }

        /// <summary>execute given node</summary>
        public void ExecuteNode(NodeBase node)
        {
            if (node is null) return;
            switch (node)
            {
                /* execute action node */
                case NodeAction actionNode:
                    actionNode.Execute();
                    break;
                
                /* execute mission node, add output prototype to buffer queue */
                case NodeMission missionNode:
                    if (activeNodes.ContainsKey(missionNode.MissionId)) return;
                    buffer.Enqueue(missionNode);
                    break;
            }
        }
    }
}