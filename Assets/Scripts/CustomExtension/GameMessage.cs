/// <summary>游戏事件类型枚举</summary>
public enum GameEventType
{
    PlayerBehaviourExample,
    PlantCrop,
    EnterRoom,
    CompleteDialogue,
    // more behaviors...
}

/// <summary>游戏消息对象</summary>
public class GameMessage
{
    public readonly GameEventType type;
    public readonly object args;
    public bool hasUsed { get; private set; }

    public GameMessage(GameEventType type, object args = null)
    {
        this.type = type;
        this.args = args;
        this.hasUsed = false;
    }

    /// <summary>使用当前消息</summary>
    public void Use() => 
        hasUsed = true;
}