using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour, IPoolObject {

    public const string DEFAULT_SUB_TYPE = "default";

    [HideInInspector] public GameObject go;
    [HideInInspector] public Transform t;

    [SerializeField] private string _subType = DEFAULT_SUB_TYPE;
    public string subType {
        get {
            return _subType;
        }
        private set {
            _subType = value;
        }
    }

    public abstract Type GetPoolObjectType();

    protected virtual void Awake() {
        go = gameObject;
        t = transform;
    }

    public virtual void OnCreate() {
        DontDestroyOnLoad(gameObject);
    }

    public virtual void OnPush() {
        gameObject.SetActive(false);
    }

    public virtual void OnPop() {
        gameObject.SetActive(true);
    }

    public void Push() {
        MPool.Push(this);
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}