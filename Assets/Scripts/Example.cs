using UnityEngine;
using RedSaw.MissionSystem;

public class Example : MonoBehaviour
{
    public class MissionLogger : IMissionSystemComponent<GameMessage>
    {
        public void OnMissionStarted(Mission<GameMessage> mission)
        {
            Debug.Log($"Mission \"{mission.id}\" started");
        }

        public void OnMissionRemoved(Mission<GameMessage> mission, bool isFinished) { }

        public void OnMissionStatusChanged(Mission<GameMessage> mission, bool isFinished)
        {
            if (isFinished)
            {
                Debug.Log($"Mission \"{mission.id}\" is finished");
                return;
            }
            Debug.Log($"Mission \"{mission.id}\" status changed: {mission.HandleStatus[0]}");
        }
    }
    
    
    void Start()
    {
        GameAPI.MissionManager.AddComponent(new MissionLogger());
    }

    MissionPrototype<GameMessage> CreateExampleProto()
    {
        /* 创建案例任务原型 */
        
        /* 任务需求是执行三次PlayerBehaviourExample */
        var missionRequire = new MissionRequireExample(GameEventType.PlayerBehaviourExample, 3);
        var requires = new MissionRequire<GameMessage>[] { missionRequire };
        var missionProto = new MissionPrototype<GameMessage>("example", requires);
        return missionProto;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameAPI.StartMission(CreateExampleProto());
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            /* 广播一条消息 */
            GameAPI.Broadcast(new GameMessage(GameEventType.PlayerBehaviourExample));
        }
    }
}