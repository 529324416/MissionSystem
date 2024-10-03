using RedSaw.MissionSystem;

public static class GameAPI
{
    /* 理论上来说这个对象应该存储于玩家的存档数据或者云端用户数据中 */
    public readonly static MissionManager<object> MissionManager;
    public readonly static MissionChainManager MissionChainManager;
    
    static GameAPI()
    {
        MissionManager = new MissionManager<object>();
        MissionChainManager = new MissionChainManager(MissionManager);
        MissionManager.AddComponent(MissionChainManager);
    }


    /// <summary>朝游戏广播一条消息</summary>
    /// <param name="message"></param>
    public static void Broadcast(GameMessage message) =>
        MissionManager.SendMessage(message);

    public static void StartMission(MissionPrototype<object> missionProto) =>
        MissionManager.StartMission(missionProto);
}

