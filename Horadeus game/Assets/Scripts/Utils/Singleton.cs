using System;
using UnityEngine;

public abstract class Singleton<T> : InternalMonoBehaviour where T : InternalMonoBehaviour {
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Inst;
    private static GameObject m_InstGO;

    public static T inst {
        get {
            if (m_ShuttingDown) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock) {
                InitIfNeeded(null);
                return m_Inst;
            }
        }
    }

    public static void InitIfNeeded(T obj) {
        if (m_Inst != null) {
            if (obj != m_Inst && obj != null) {
                Debug.Log("[Singleton] Destroying 2nd object of type " + obj.GetType().Name);
                Destroy(obj.gameObject);
            }
            return;
        }

        Type type = typeof(T);

        if (obj != null) {
            m_Inst = obj;
        } else {
            m_Inst = (T)FindObjectOfType(type);
        }

        if (m_Inst == null) {
            m_InstGO = new GameObject();
            m_Inst = m_InstGO.AddComponent<T>();
        } else {
            m_InstGO = m_Inst.gameObject;
        }

        if (m_Inst == null) {
            Debug.LogError("Unable to load singleton of type " + type.Name);
            Debug.Break();
            return;
        }

        m_InstGO.name = "[Singleton] " + type.Name;

        DontDestroyOnLoad(m_InstGO);
        m_Inst.Init();
    }

    private void OnApplicationQuit() {
        if (!m_ShuttingDown) {
            Shutdown();
            m_ShuttingDown = true;
            Debug.Log("[Singleton] OnApplicationQuit");
        }
    }

    private void OnDestroy() {
        if (!m_ShuttingDown && this == m_Inst) {
            Shutdown();
            m_ShuttingDown = true;
            Debug.Log("[Singleton] OnDestroy Shutting down");
        }
    }

    protected abstract void Shutdown();
}

public abstract class SingletonFromResources<T> : InternalMonoBehaviour where T : InternalMonoBehaviour {
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Inst;
    private static GameObject m_InstGO;

    public static T inst {
        get {
            if (m_ShuttingDown) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock) {
                InitIfNeeded(null);
                return m_Inst;
            }
        }
    }

    public static void InitIfNeeded(T obj) {
        if (m_Inst != null) {
            if (obj != m_Inst && obj != null) {
                Debug.Log("[Singleton] Destroying 2nd object of type " + obj.GetType().Name);
                Destroy(obj.gameObject);
            }
            return;
        }

        Type type = typeof(T);

        if (obj != null) {
            m_Inst = obj;
        } else {
            m_Inst = (T)FindObjectOfType(type);
        }

        if (m_Inst == null) {
            m_InstGO = Instantiate(Resources.Load(type.Name) as GameObject);
            m_Inst = m_InstGO.GetComponent<T>();
        } else {
            m_InstGO = m_Inst.gameObject;
        }

        if (m_Inst == null) {
            Debug.LogError("Unable to load singleton of type " + type.Name);
            Debug.Break();
            return;
        }

        m_InstGO.name = "[Singleton] " + type.Name;

        DontDestroyOnLoad(m_InstGO);
        m_Inst.Init();
    }


    private void OnApplicationQuit() {
        if (!m_ShuttingDown) {
            Shutdown();
            m_ShuttingDown = true;
        }
    }

    private void OnDestroy() {
        if (!m_ShuttingDown && this == m_Inst) {
            Shutdown();
            m_ShuttingDown = true;
        }
    }

    protected abstract void Shutdown();
}

public abstract class SingletonScriptableObject<T> : InternalScriptableObject where T : InternalScriptableObject {
    private static object m_Lock = new object();
    private static T m_Inst;

    public static T inst {
        get {
            lock (m_Lock) {
                InitIfNeeded(null);
                return m_Inst;
            }
        }
    }

    public static void InitIfNeeded(T obj) {
        if (m_Inst != null) {
            return;
        }

        if (obj != null) {
            m_Inst = obj;
        } else {
            m_Inst = Resources.Load<T>(typeof(T).Name);
        }

        if (m_Inst == null) {
            Debug.LogError("Unable to load singleton of type " + typeof(T).Name);
            Debug.Break();
            return;
        }

        m_Inst.Init();
    }

}

public abstract class InternalScriptableObject : ScriptableObject {
    public abstract void Init();
}

public abstract class InternalMonoBehaviour : MonoBehaviour {
    public abstract void Init();
}