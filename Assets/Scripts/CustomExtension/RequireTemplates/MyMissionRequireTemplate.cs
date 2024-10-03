using System;
using ParadoxNotion.Design;
using RedSaw.Editor;
using RedSaw.MissionSystem;
using UnityEngine;

[Name("案例模板"), Description("基础任务模板")]
public class MyMissionRequireTemplate : MissionRequireTemplate
{
    [SerializeField] private string eventType;
    [SerializeField] private int count;
    
    public override bool CheckMessage(object message)
    {
        if (message is not GameMessage gameMessage) return false;
        return gameMessage.type.ToString() == eventType;
    }

    public class Handle : MissionRequireTemplateHandle
    {
        private readonly MyMissionRequireTemplate require;
        private int count;

        public Handle(MyMissionRequireTemplate require) : base(require)
        {
            this.require = require;
        }
        protected override bool UseMessage(object message)
        {
            return ++count == require.count;
        }
    }

#if UNITY_EDITOR

    public override string Summary
    {
        get
        {
            return $"监听<b><size=12><color=#fffde3> \"{eventType}\" </color></size></b>事件<size=12><b><color=#b1d480> {count} </color></b></size>次";
        }
    }

    protected override void OnInspectorGUI()
    {
        DropdownMenu.MakeMenu("事件类型", eventType, Enum.GetNames(typeof(GameEventType)), result => eventType = result);
        count = UnityEditor.EditorGUILayout.IntField("数量", count);
        count = Mathf.Max(1, count);
    }
#endif
}