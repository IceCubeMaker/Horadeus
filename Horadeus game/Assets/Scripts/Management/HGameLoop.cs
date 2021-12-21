using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HGameLoop : Singleton<HGameLoop>
{

    public static PrioritizedEvent PreUpdate = new PrioritizedEvent();
    public static PrioritizedEvent Update = new PrioritizedEvent();
    public static PrioritizedEvent LateUpdate = new PrioritizedEvent();
    public static PrioritizedEvent FixedUpdate = new PrioritizedEvent();
    public static PrioritizedEvent NetworkFixedUpdate = new PrioritizedEvent();

    public const int DEFAULT_PRIORITY = int.MaxValue / 2;

    public override void Init()
    {

    }

    protected override void Shutdown()
    {

    }

    public void InternalAwake()
    {

    }

    public void InternalStart()
    {

    }

    public void InternalPreUpdate()
    {
        PreUpdate.Invoke();
    }

    public void InternalUpdate()
    {
        Update.Invoke();
    }

    public void InternalLateUpdate()
    {
        LateUpdate.Invoke();
    }

    public void InternalFixedUpdate()
    {
        FixedUpdate.Invoke();
    }

}

public class PrioritizedEvent
{

    private SortedList<int, List<Action>> actions;
    private Dictionary<Action, int> typedCache;

    private Stack<RegisterRequest> pendingRegisterRequests;
    private Stack<UnregisterRequest> pendingUnregesterRequests;

    public PrioritizedEvent()
    {
        actions = new SortedList<int, List<Action>>();
        typedCache = new Dictionary<Action, int>();
        pendingRegisterRequests = new Stack<RegisterRequest>();
        pendingUnregesterRequests = new Stack<UnregisterRequest>();
    }

    public void Invoke()
    {
        while (pendingUnregesterRequests.Count > 0)
        {
            UnregisterRequest request = pendingUnregesterRequests.Pop();
            Internal_Unregister(request.action);
        }

        while (pendingRegisterRequests.Count > 0)
        {
            RegisterRequest request = pendingRegisterRequests.Pop();
            Internal_Register(request.priority, request.action);
        }

        foreach (var item in actions)
        {
            for (int i = item.Value.Count - 1; i >= 0; i--)
            {
                item.Value[i]?.Invoke();
            }
        }
    }

    private void Internal_Register(int priority, Action action)
    {
        if (!actions.ContainsKey(priority))
        {
            actions.Add(priority, new List<Action>());
        }

        actions[priority].Add(action);
        typedCache.Add(action, priority);
    }

    private void Internal_Unregister(Action action)
    {
        var list = actions[typedCache[action]];
        list.Remove(action);
    }

    public void Register(Action action)
    {
        pendingRegisterRequests.Push(new RegisterRequest() {
            action = action,
            priority = HGameLoop.DEFAULT_PRIORITY
        });
    }

    public void Register(int priorityOffset, Action action)
    {
        int newPriority = (HGameLoop.DEFAULT_PRIORITY + priorityOffset);
        pendingRegisterRequests.Push(new RegisterRequest() {
            action = action,
            priority = newPriority
        });
    }

    public void Unregister(Action action)
    {
        pendingUnregesterRequests.Push(new UnregisterRequest() {
            action = action
        });
    }

    public struct RegisterRequest
    {
        public Action action;
        public int priority;
    }

    public struct UnregisterRequest
    {
        public Action action;
    }

}

public class PrioritizedEvent<T>
{

    private SortedList<int, List<Action<T>>> actions;
    private Dictionary<Action<T>, int> typedCache;

    public PrioritizedEvent()
    {
        actions = new SortedList<int, List<Action<T>>>();
        typedCache = new Dictionary<Action<T>, int>();
    }

    public void Invoke(T obj)
    {
        foreach (var item in actions)
        {
            foreach (var action in item.Value)
            {
                action?.Invoke(obj);
            }
        }
    }

    public void Add(Action<T> action)
    {
        if (!actions.ContainsKey(HGameLoop.DEFAULT_PRIORITY))
        {
            actions.Add(HGameLoop.DEFAULT_PRIORITY, new List<Action<T>>());
        }

        actions[HGameLoop.DEFAULT_PRIORITY].Add(action);
        typedCache.Add(action, HGameLoop.DEFAULT_PRIORITY);
    }

    public void Add(int priorityOffset, Action<T> action)
    {

        int newPriority = (HGameLoop.DEFAULT_PRIORITY + priorityOffset);

        if (!actions.ContainsKey(newPriority))
        {
            actions.Add(newPriority, new List<Action<T>>());
        }

        actions[newPriority].Add(action);
        typedCache.Add(action, newPriority);
    }

    public void Remove(Action<T> action)
    {
        var list = actions[typedCache[action]];
        list.Remove(action);
    }

}

public class PrioritizedEvent<T, T1>
{

    private SortedList<int, List<Action<T, T1>>> actions;
    private Dictionary<Action<T, T1>, int> typedCache;

    public PrioritizedEvent()
    {
        actions = new SortedList<int, List<Action<T, T1>>>();
        typedCache = new Dictionary<Action<T, T1>, int>();
    }

    public void Invoke(T obj, T1 obj1)
    {
        foreach (var item in actions)
        {
            foreach (var action in item.Value)
            {
                action?.Invoke(obj, obj1);
            }
        }
    }

    public void Add(Action<T, T1> action)
    {
        if (!actions.ContainsKey(HGameLoop.DEFAULT_PRIORITY))
        {
            actions.Add(HGameLoop.DEFAULT_PRIORITY, new List<Action<T, T1>>());
        }

        actions[HGameLoop.DEFAULT_PRIORITY].Add(action);
        typedCache.Add(action, HGameLoop.DEFAULT_PRIORITY);
    }

    public void Add(int priorityOffset, Action<T, T1> action)
    {

        int newPriority = (HGameLoop.DEFAULT_PRIORITY + priorityOffset);

        if (!actions.ContainsKey(newPriority))
        {
            actions.Add(newPriority, new List<Action<T, T1>>());
        }

        actions[newPriority].Add(action);
        typedCache.Add(action, newPriority);
    }

    public void Remove(Action<T, T1> action)
    {
        var list = actions[typedCache[action]];
        list.Remove(action);
    }

}

public struct GameLoopCallback
{

}

public interface IGameLoopListener
{
    void Init();
    void InternalAwake();
    void InternalStart();
    void InternalUpdate();
    void InternalFixedUpdate();
    List<IGameLoopListener> childListeners { get; }
    GameLoopPriority priority { get; }
}

public enum GameLoopEventType
{
    INIT,
    AWAKE,
    START,
    UPDATE,
    FIXED_UPDATE,
}

public struct GameLoopPriority
{
    public GameLoopStagePriority init;
    public GameLoopStagePriority awake;
    public GameLoopStagePriority start;
    public GameLoopStagePriority update;
    public GameLoopStagePriority fixed_update;

    public GameLoopPriority(byte basePriority)
    {
        init = new GameLoopStagePriority(GameLoopEventType.INIT, basePriority);
        awake = new GameLoopStagePriority(GameLoopEventType.AWAKE, basePriority);
        start = new GameLoopStagePriority(GameLoopEventType.START, basePriority);
        update = new GameLoopStagePriority(GameLoopEventType.UPDATE, basePriority);
        fixed_update = new GameLoopStagePriority(GameLoopEventType.FIXED_UPDATE, basePriority);
    }

    public void Set(GameLoopEventType stage, byte priority)
    {
        switch (stage)
        {
            case GameLoopEventType.INIT:
                init.priority = priority;
                break;
            case GameLoopEventType.START:
                awake.priority = priority;
                break;
            case GameLoopEventType.AWAKE:
                start.priority = priority;
                break;
            case GameLoopEventType.UPDATE:
                update.priority = priority;
                break;
            case GameLoopEventType.FIXED_UPDATE:
                fixed_update.priority = priority;
                break;
        }
    }

}

public struct GameLoopStagePriority
{
    public GameLoopEventType stage;
    public byte priority;

    public GameLoopStagePriority(GameLoopEventType _stage, byte _priority)
    {
        stage = _stage;
        priority = _priority;
    }
}

/*
public class GameLoopPriorityComparer : Comparer<GameLoopStagePriority> {
    public override int Compare(GameLoopStagePriority x, GameLoopStagePriority y) {
        if(x.priority)
    }
}
*/

public abstract class GameLoopListener : MonoBehaviour, IGameLoopListener
{

    private GameLoopPriority? m_Priority;
    public GameLoopPriority priority
    {
        get {
            if (m_Priority == null)
            {
                m_Priority = generatedPriority;
            }

            return (GameLoopPriority)m_Priority;
        }
    }
    protected abstract GameLoopPriority generatedPriority { get; }

    private List<IGameLoopListener> m_childListeners;
    public List<IGameLoopListener> childListeners
    {
        get {
            return m_childListeners;
        }
    }

    private SortedList<byte, IGameLoopListener> childSortedList;

    protected void AddChildListener(IGameLoopListener child)
    {
        m_childListeners.Add(child);
    }

    public virtual void Init()
    {
        childSortedList = new SortedList<byte, IGameLoopListener>();
    }

    public virtual void InternalAwake()
    {

    }

    public virtual void InternalFixedUpdate()
    {

    }

    public virtual void InternalStart()
    {

    }

    public virtual void InternalUpdate()
    {

    }

}

public delegate void GameEventDelegate();

public class GameLoopEvent
{
    public GameLoopEventType type;
    private GameEventDelegate eventDelegate;

    public static GameLoopEvent operator +(GameLoopEvent gameLoopEvent, GameEventDelegate callback)
    {
        gameLoopEvent.eventDelegate += callback;
        return gameLoopEvent;
    }

}

/*

GameLoop.Update +=


*/