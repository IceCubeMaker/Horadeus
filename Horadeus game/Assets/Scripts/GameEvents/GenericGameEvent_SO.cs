using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent<T> : ScriptableObject, IGameEvent<T>
{
    public static List<IListener<T>> listenersVar;

    private void OnEnable()
    {
        listenersVar = new List<IListener<T>>();
    }

    public void AddListener(IListener<T> listener)
    {
        if (!listenersVar.Contains(listener)) listenersVar.Add(listener);
    }

    public void RemoveListener(IListener<T> listener)
    {
        if (listenersVar.Contains(listener)) listenersVar.Remove(listener);
    }

    public virtual void Raise(T variable)
    {
        for (int i = listenersVar.Count - 1; i >= 0; i--)
        {
            listenersVar[i].OnRaised(variable);
        }
    }
}

public interface IGameEvent<T>
{
    void AddListener(IListener<T> listener);
    void RemoveListener(IListener<T> listener);
    void Raise(T variable);
}
public interface IGameEvent
{
    void AddListener(IListener listener);
    void RemoveListener(IListener listener);
    void Raise();
}

public interface IListener<T>
{
    void OnRaised(T t);
}
public interface IListener
{
    void OnRaised();
}