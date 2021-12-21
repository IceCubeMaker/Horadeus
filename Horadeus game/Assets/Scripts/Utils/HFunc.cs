using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFunc : Singleton<HFunc>
{

    private static List<FuncTimer> timerFunctions;
    private static List<FuncTick> tickFunctions;

    private static object m_LockTimer = new object();
    private static object m_LockTick = new object();

    public override void Init()
    {
        timerFunctions = new List<FuncTimer>();
        tickFunctions = new List<FuncTick>();

        HGameLoop.PreUpdate.Register(InternalPreUpdate);
        HGameLoop.Update.Register(InternalUpdate);
        HGameLoop.LateUpdate.Register(InternalLateUpdate);
        HGameLoop.FixedUpdate.Register(InternalFixedUpdate);
    }

    protected override void Shutdown()
    {
        HGameLoop.PreUpdate.Unregister(InternalPreUpdate);
        HGameLoop.Update.Unregister(InternalUpdate);
        HGameLoop.LateUpdate.Unregister(InternalLateUpdate);
        HGameLoop.FixedUpdate.Unregister(InternalFixedUpdate);
    }

    private void InternalPreUpdate()
    {
        lock (m_LockTick)
        {
            for (int i = 0; i < tickFunctions.Count; i++)
            {
                tickFunctions[i].Update(FuncUpdateType.PRE_UPDATE);
            }
        }
    }

    private void InternalUpdate()
    {
        lock (m_LockTimer)
        {
            for (int i = 0; i < timerFunctions.Count; i++)
            {
                if (!timerFunctions[i].isFixed)
                {
                    timerFunctions[i].Update();
                }
            }
        }

        lock (m_LockTick)
        {
            for (int i = 0; i < tickFunctions.Count; i++)
            {
                tickFunctions[i].Update(FuncUpdateType.UPDATE);
            }
        }
    }

    private void InternalLateUpdate()
    {
        lock (m_LockTick)
        {
            for (int i = 0; i < tickFunctions.Count; i++)
            {
                tickFunctions[i].Update(FuncUpdateType.LATE_UPDATE);
            }
        }
    }

    private void InternalFixedUpdate()
    {
        lock (m_LockTimer)
        {
            for (int i = 0; i < timerFunctions.Count; i++)
            {
                if (timerFunctions[i].isFixed)
                {
                    timerFunctions[i].FixedUpdate();
                }
            }
        }

        lock (m_LockTick)
        {
            for (int i = 0; i < tickFunctions.Count; i++)
            {
                tickFunctions[i].Update(FuncUpdateType.FIXED_UPDATE);
            }
        }
    }

    public static void Wait(Action action, float delay, bool unscaledDeltaTime = false)
    {
        inst.StartCoroutine(inst.WaitCoroutine(action, delay, unscaledDeltaTime));
    }

    public static void Wait(Action action, int ticks)
    {
        inst.StartCoroutine(inst.WaitCoroutine(action, ticks));
    }

    private IEnumerator WaitCoroutine(Action action, float delay, bool unscaledDeltaTime)
    {
        if (unscaledDeltaTime)
        {
            yield return new WaitForSecondsRealtime(delay);
        } else
        {
            yield return new WaitForSeconds(delay);
        }
        if (action != null)
        {
            action();
        }
    }

    private IEnumerator WaitCoroutine(Action action, int ticks)
    {
        for (int i = 0; i < ticks; i++)
        {
            yield return null;
        }

        if (action != null)
        {
            action();
        }
    }

    #region Wrappers

    public static FuncTick StartTick(string funcName, FuncUpdateType updateType, Action action, Func<bool> _destroyCheckFunc = null, Action _onStopCallback = null)
    {
        InitIfNeeded(null);

        FuncTick func = new FuncTick(funcName, updateType, action, _destroyCheckFunc, _onStopCallback);
        lock (m_LockTick)
        {
            tickFunctions.Add(func);
        }

        return func;
    }

    public static FuncTimer StartTimer(string funcName, float period, bool isUnscaledTime, Action action)
    {
        InitIfNeeded(null);

        FuncTimer func = new FuncTimer(funcName, period, action, isUnscaledTime, false);
        lock (m_LockTimer)
        {
            timerFunctions.Add(func);
        }
        return func;
    }

    public static void StopTimer(string funcName)
    {
        lock (m_LockTimer)
        {
            for (int i = timerFunctions.Count - 1; i >= 0; i--)
            {
                if (timerFunctions[i].funcName == funcName)
                {
                    timerFunctions[i].Stop(false);
                    timerFunctions.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public static void StopAllTimers(string funcName)
    {
        lock (m_LockTimer)
        {
            for (int i = timerFunctions.Count - 1; i >= 0; i--)
            {
                if (timerFunctions[i].funcName == funcName)
                {
                    timerFunctions[i].Stop(false);
                    timerFunctions.RemoveAt(i);
                }
            }
        }
    }

    public static void StopTick(string funcName)
    {
        lock (m_LockTick)
        {
            for (int i = tickFunctions.Count - 1; i >= 0; i--)
            {
                if (tickFunctions[i].funcName == funcName)
                {
                    tickFunctions[i].Stop(false);
                    tickFunctions.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public static void StopAllTick(string funcName)
    {
        lock (m_LockTick)
        {
            for (int i = tickFunctions.Count - 1; i >= 0; i--)
            {
                if (tickFunctions[i].funcName == funcName)
                {
                    tickFunctions[i].Stop(false);
                    tickFunctions.RemoveAt(i);
                }
            }
        }
    }

    public static void StopTimer(FuncTimer func)
    {
        lock (m_LockTimer)
        {
            timerFunctions.Remove(func);
        }
    }

    public static void StopTick(FuncTick func)
    {
        lock (m_LockTick)
        {
            tickFunctions.Remove(func);
        }
    }

    #endregion

}

public class FuncTick
{

    public FuncUpdateType updateType { get; private set; }
    private Action action;
    private Func<bool> destroyCheckFunc;
    private Action onStopCallback;
    public string funcName { get; private set; }

    public FuncTick(string _funcName, FuncUpdateType type, Action _callback, Func<bool> _destroyCheckFunc = null, Action _onStopCallback = null)
    {
        funcName = _funcName;
        updateType = type;
        action = _callback;
        destroyCheckFunc = _destroyCheckFunc;
        onStopCallback = _onStopCallback;
    }

    public void Stop(bool removeSelf = true)
    {
        onStopCallback?.Invoke();

        if (removeSelf)
        {
            HFunc.StopTick(this);
        }
    }

    public void Update(FuncUpdateType type)
    {
        if (updateType == type)
        {
            if (action != null)
            {
                action();
            }

            if (destroyCheckFunc != null && destroyCheckFunc())
            {
                Stop();
                return;
            }
        }
    }

}

public enum FuncUpdateType
{
    PRE_UPDATE,
    UPDATE,
    LATE_UPDATE,
    FIXED_UPDATE
}

public class FuncTimer
{

    private float timer;
    private float period;
    private Action callback;
    private Func<bool> destroyCheck;
    private Action onStopCallback;
    private bool checkDestroyOnlyOnTimerTick;
    public bool useUnscaledTime { get; private set; }
    public bool isFixed { get; private set; }
    public string funcName { get; private set; }

    public FuncTimer(string _funcName, float _period, Action _callback, bool _useUnscaledTime, bool _isFixed)
    {
        funcName = _funcName;
        period = _period;
        timer = period;
        callback = _callback;
        useUnscaledTime = _useUnscaledTime;
        isFixed = _isFixed;
    }

    public FuncTimer SetDestroyFunc(Func<bool> _destroyCheck, bool _checkDestroyOnlyOnTimerTick)
    {
        destroyCheck = _destroyCheck;
        checkDestroyOnlyOnTimerTick = _checkDestroyOnlyOnTimerTick;
        return this;
    }

    public FuncTimer SetOnStop(Action _onStopCallback)
    {
        onStopCallback = _onStopCallback;
        return this;
    }

    public void Stop(bool removeSelf = true)
    {
        if (onStopCallback != null)
        {
            onStopCallback();
        }

        if (removeSelf)
        {
            HFunc.StopTimer(this);
        }
    }

    public void Update()
    {
        if (destroyCheck != null && !checkDestroyOnlyOnTimerTick && destroyCheck())
        {
            Stop();
            return;
        }

        if (useUnscaledTime)
        {
            timer -= Time.unscaledDeltaTime;
        } else
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            callback?.Invoke();

            if (destroyCheck != null && checkDestroyOnlyOnTimerTick && destroyCheck())
            {
                Stop();
                return;
            } else
            {
                timer += period;
            }
        }
    }

    public void FixedUpdate()
    {
        if (destroyCheck != null && !checkDestroyOnlyOnTimerTick && destroyCheck())
        {
            Stop();
            return;
        }

        timer -= Time.fixedDeltaTime;

        if (timer <= 0)
        {
            callback?.Invoke();

            if (destroyCheck != null && checkDestroyOnlyOnTimerTick && destroyCheck())
            {
                Stop();
                return;
            } else
            {
                timer += period;
            }
        }
    }

}

public static class MCFuncExt
{
    public static void Wait(this MonoBehaviour m, Action action, float delay)
    {
        m.StartCoroutine(WaitCoroutine(action, delay));
    }

    private static IEnumerator WaitCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (action != null)
        {
            action();
        }
    }

    public static void WaitRealtime(this MonoBehaviour m, Action action, float delay)
    {
        m.StartCoroutine(WaitRealtimeCoroutine(action, delay));
    }

    private static IEnumerator WaitRealtimeCoroutine(Action action, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (action != null)
        {
            action();
        }
    }

    public static void Wait(this MonoBehaviour m, Action action, int ticks)
    {
        m.StartCoroutine(WaitCoroutine(action, ticks));
    }

    private static IEnumerator WaitCoroutine(Action action, int ticks)
    {
        for (int i = 0; i < ticks; i++)
        {
            yield return null;
        }

        if (action != null)
        {
            action();
        }
    }
}