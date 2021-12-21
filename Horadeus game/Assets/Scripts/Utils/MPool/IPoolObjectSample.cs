using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPoolObjectSample : MonoBehaviour, IPoolObject {

    [SerializeField] private string m_SubType = MPool.DEFAULT_SUB_TYPE;
    public string subType => m_SubType;

    public GameObject GetGameObject() {
        return gameObject;
    }

    public abstract Type GetPoolObjectType();

    public virtual void OnCreate() { }

    public virtual void OnPop() { }

    public virtual void OnPush() { }

}
