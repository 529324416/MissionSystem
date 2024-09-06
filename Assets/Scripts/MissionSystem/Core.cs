using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RedSaw.MissionSystem
{
    /// <summary>任务原型对象</summary>
    public class MissionPrototype<T>
    {
        public readonly string id;
        public readonly MissionProperty property; 
        public readonly MissionRequire<T>[] requires;
        public readonly MissionRequireMode requireMode;
        public readonly bool isSingleRequire;
        private readonly MissionReward[] rewards;
        
        /// <summary>
        /// 初始化任务原型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requires"></param>
        /// <param name="rewards"></param>
        /// <param name="requireMode"></param>
        /// <param name="property"></param>
        /// <exception cref="Exception"></exception>
        public MissionPrototype(string id, [DisallowNull] MissionRequire<T>[] requires, MissionReward[] rewards = null, MissionRequireMode requireMode = default, MissionProperty property = null)
        {
            /* check if mission id is valid */
            if (string.IsNullOrEmpty(id)) 
                throw new Exception("mission id cannot be null or empty");
            this.id = id;
            
            /* check if require array is valid */
            if (requires == null || requires.Length == 0)
                throw new Exception("mission requires cannot be null or empty");

            this.requires = requires;
            this.rewards = rewards;
            this.requireMode = requireMode;
            this.property = property;
            
            this.isSingleRequire = requires.Length == 1;
        }

        /// <summary>兑现所有的奖励</summary>
        public void ApplyReward()
        {
            if (rewards is null || rewards.Length == 0) return;
            foreach (var reward in rewards)
                reward.ApplyReward();
        }
    }

    /// <summary>任务需求模式</summary>
    public enum MissionRequireMode
    {
        All,
        Any
    }

    /// <summary>任务奖励</summary>
    public abstract class MissionReward
    {
        /// <summary>兑现玩家的奖励</summary>
        public abstract void ApplyReward();
    }

    /// <summary>任务附加属性描述</summary>
    public abstract class MissionProperty { }
    
    /// <summary>决定玩家具体要执行的行为</summary>
    /// <typeparam name="T">消息类型</typeparam>
    [System.Serializable]
    public abstract class MissionRequire<T>
    {
        /// <summary>检查给定的消息是否对当前需求有效</summary>
        /// <param name="message">目标消息</param>
        /// <returns>是否有效</returns>
        public abstract bool CheckMessage(T message);

        /// <summary>创建当前需求的状态记录柄</summary>
        /// <returns></returns>
        public MissionRequireHandle<T> CreateHandle()
        {
            var _handleType = GetType().GetNestedType("Handle");
            if (_handleType == null)
                throw new Exception($"{GetType()} has not defined Handle");

            return (MissionRequireHandle<T>)Activator.CreateInstance(_handleType, this);
        }
    }

    /// <summary>记录玩家当前的某个任务需求状态</summary>
    public abstract class MissionRequireHandle<T>
    {
        private readonly MissionRequire<T> _require;
        
        protected MissionRequireHandle(MissionRequire<T> require)
        {
            _require = require;
        }
        
        /// <summary>发送一条消息给玩家</summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        public bool SendMessage(T message, out bool hasStatusChanged)
        {
            hasStatusChanged = false;
            if (!_require.CheckMessage(message)) return false;
            hasStatusChanged = true;
            return UseMessage(message);
        }

        /// <summary>应用某条消息并返回当前需求是否已经完成</summary>
        /// <param name="message">目标消息</param>
        /// <returns></returns>
        protected abstract bool UseMessage(T message);
    }

    /// <summary>任务</summary>
    /// <typeparam name="T"></typeparam>
    public class Mission<T>
    {
        private readonly MissionPrototype<T> proto;
        private readonly MissionRequireHandle<T>[] handles;

        private readonly List<MissionRequireHandle<T>> _unfinishedHandles =
            new List<MissionRequireHandle<T>>();

        public string id => proto.id;
        public MissionProperty property => proto.property;

        /// <summary>获取任务的进度状态</summary>
        public string[] HandleStatus
        {
            get
            {
                var status = new string[handles.Length];
                for (var i = 0; i < handles.Length; i++)
                    status[i] = handles[i].ToString();
                return status;
            }
        }
        
        public Mission(MissionPrototype<T> proto)
        {
            this.proto = proto;
            handles = proto.requires.Select(r => r.CreateHandle()).ToArray();
            if (!proto.isSingleRequire)
                _unfinishedHandles.AddRange(handles);
        }

        /// <summary>兑现任务的奖励</summary>
        public void ApplyReward() => proto.ApplyReward();

        /// <summary>向任务发送玩家行为消息并检查任务是否完成以及是否发生状态变化</summary>
        /// <param name="message">玩家行为消息</param>
        /// <param name="hasStatusChanged">任务是否产生状态变化</param>
        /// <returns>任务是否完成</returns>
        public bool SendMessage(T message, out bool hasStatusChanged) =>
            proto.isSingleRequire
                ? _SendMessage_SingleRequire(message, out hasStatusChanged)
                : _SendMessage_MultiRequire(message, out hasStatusChanged);

        /// <summary>单需求的任务处理</summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        private bool _SendMessage_SingleRequire(T message, out bool hasStatusChanged) =>
            handles[0].SendMessage(message, out hasStatusChanged);

        /// <summary>多需求任务处理</summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        private bool _SendMessage_MultiRequire(T message, out bool hasStatusChanged)
        {
            hasStatusChanged = false;
            var queueToRemove = new Queue<MissionRequireHandle<T>>();
            
            /* update all require handles */
            foreach (var requireHandle in _unfinishedHandles)
            {
                if (!requireHandle.SendMessage(message, out var _hasStatusChanged))
                {
                    hasStatusChanged |= _hasStatusChanged;
                    continue;
                }
                hasStatusChanged = true;
                if (proto.requireMode == MissionRequireMode.Any) return true;
                queueToRemove.Enqueue(requireHandle);
            }
            
            /* remove completed requries */
            while (queueToRemove.Count > 0)
            {
                var handle = queueToRemove.Dequeue();
                _unfinishedHandles.Remove(handle);
            }

            /* check if all requires have been completed */
            return _unfinishedHandles.Count == 0;
        }
    }

    /// <summary>任务管理器</summary>
    /// <typeparam name="T">消息类型</typeparam>
    public class MissionManager<T>
    {
        private readonly Dictionary<string, Mission<T>> allMissions = new Dictionary<string, Mission<T>>();
        private readonly List<IMissionSystemComponent<T>> components = new List<IMissionSystemComponent<T>>();

        /// <summary>启动目标任务</summary>
        /// <param name="proto"></param>
        /// <returns></returns>
        public bool StartMission(MissionPrototype<T> proto)
        {
            if (proto is null || allMissions.ContainsKey(proto.id)) return false;
            var mission = new Mission<T>(proto);
            allMissions.Add(proto.id, mission);
            
            /* 通知所有的组件任务启动了 */
            foreach (var component in components)
                component.OnMissionStarted(mission);
            return true;
        }

        /// <summary>查询所有符合条件的任务（条件为空时返回所有任务）</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Mission<T>[] GetMissions(Func<MissionProperty, bool> condition = null)
        {
            return condition is null
                ? allMissions.Values.ToArray()
                : allMissions.Values.Where(m => condition(m.property)).ToArray();
        }

        /// <summary>查找对应id的任务</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Mission<T> GetMission(string id)
        {
            return string.IsNullOrEmpty(id) ? null : allMissions.GetValueOrDefault(id, null);
        }

        /// <summary>移除目标任务</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveMission(string id)
        {
            if (!allMissions.Remove(id, out var mission)) return false;
            foreach (var component in components)
                component.OnMissionRemoved(mission, false);
            return true;
        }
        
        /// <summary>向任务系统发送消息以驱动任务系统</summary>
        /// <param name="message"></param>
        public void SendMessage(T message)
        {
            if (allMissions.Count == 0) return;
            var queueToRemove = new Queue<Mission<T>>(); 
            foreach (var mission in allMissions.Values)
            {
                if (!mission.SendMessage(message, out var hasStatusChanged))
                {
                    if (hasStatusChanged) _OnMissionStatusChanged(mission, false);
                    continue;
                }

                _OnMissionStatusChanged(mission, true);
                mission.ApplyReward();
                queueToRemove.Enqueue(mission);
            }
            
            /* remove completed missions */
            while (queueToRemove.Count > 0)
            {
                var mission = queueToRemove.Dequeue();
                allMissions.Remove(mission.id);
                
                /* inform all componetns that target mission has been removed */
                foreach (var component in components)
                    component.OnMissionRemoved(mission, true);
            }
        }
        
        /// <summary>添加任务系统组件</summary>
        /// <param name="component"></param>
        public bool AddComponent(IMissionSystemComponent<T> component)
        {
            if (component is null || components.Contains(component)) return false;
            components.Add(component);
            return true;
        }

        /// <summary>移除任务组件</summary>
        /// <param name="component"></param>
        public bool RemoveComponent(IMissionSystemComponent<T> component)
        {
            return component is not null && components.Remove(component);
        }

        private void _OnMissionStatusChanged(Mission<T> mission, bool isFinished)
        {
            foreach (var component in components)
                component.OnMissionStatusChanged(mission, isFinished);
        }
    }


    /// <summary>任务系统组件接口</summary>
    public interface IMissionSystemComponent<T>
    {
        /// <summary>任务启动时触发该函数</summary>
        /// <param name="mission"></param>
        public void OnMissionStarted(Mission<T> mission);

        /// <summary>任务被移除时触发该函数</summary>
        /// <param name="mission"></param>
        /// <param name="isFinished">任务是否已经完成</param>
        public void OnMissionRemoved(Mission<T> mission, bool isFinished);

        /// <summary>任务状态变化时触发该函数</summary>
        /// <param name="mission"></param>
        /// <param name="isFinished"></param>
        public void OnMissionStatusChanged(Mission<T> mission, bool isFinished);
    }
    
}