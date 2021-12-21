using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "MicroCrew/MPool", fileName = "MPool")]
public class MPool : SingletonScriptableObject<MPool> {

    public List<GameObject> prefabs;

    [HideInInspector] public Dictionary<Type, Dictionary<string, List<IPoolObject>>> objects;
    [HideInInspector] public Dictionary<Type, Dictionary<string, IPoolObject>> objPrefabs;

    public override void Init() {
        objects = new Dictionary<Type, Dictionary<string, List<IPoolObject>>>();
        objPrefabs = new Dictionary<Type, Dictionary<string, IPoolObject>>();

        AddPrefabs(prefabs.Select(x => x.GetComponent<IPoolObject>()).ToList());
    }

    public void AddPrefabs(List<IPoolObject> prefabsToAdd) {
        for (int i = 0; i < prefabsToAdd.Count; i++) {
            Type t = prefabsToAdd[i].GetPoolObjectType();
            string subType = prefabsToAdd[i].subType;

            if (!objPrefabs.ContainsKey(t)) {
                objPrefabs.Add(t, new Dictionary<string, IPoolObject>());
            }

            if (!objPrefabs[t].ContainsKey(subType)) {
                objPrefabs[t].Add(subType, prefabsToAdd[i]);
            }
        }
    }

    public IPoolObject CreateNew(IPoolObject prefab) {
        // Debug.Log(prefab.GetGameObject() == null);
        IPoolObject newInstance = MonoBehaviour.Instantiate(prefab.GetGameObject()).GetComponent<IPoolObject>();
        return newInstance;
    }

    private T Internal_Get<T>(string subType) where T : IPoolObject {
        Type targetType = typeof(T);

        if (!objPrefabs.ContainsKey(targetType)) {
            targetType = targetType.BaseType;
        }

        if (!objects.ContainsKey(targetType)) {
            objects.Add(targetType, new Dictionary<string, List<IPoolObject>>());
        }

        if (!objects[targetType].ContainsKey(subType)) {
            objects[targetType].Add(subType, new List<IPoolObject>());
        }

        if (objects[targetType][subType].Count > 0) {
            T obj = (T)objects[targetType][subType][0];
            objects[targetType][subType].RemoveAt(0);
            obj.OnPop();
            return obj;
        } else {
            IPoolObject newObj = CreateNew(objPrefabs[targetType][subType]);
            newObj.OnCreate();
            newObj.OnPop();
            return (T)newObj;
        }
    }

    private void Internal_Push(IPoolObject obj) {
        Type objType = obj.GetPoolObjectType();

        if (!objects.ContainsKey(objType)) {
            objects.Add(objType, new Dictionary<string, List<IPoolObject>>());
        }

        if (!objects[objType].ContainsKey(obj.subType)) {
            objects[objType].Add(obj.subType, new List<IPoolObject>());
        }

        objects[objType][obj.subType].Add(obj);
        obj.OnPush();
    }

    public static T Get<T>(string subType = null) where T : IPoolObject {
        InitIfNeeded(null);

        if (subType == null) {
            subType = MPool.DEFAULT_SUB_TYPE;
        }
        return inst.Internal_Get<T>(subType);
    }

    public static List<T> GetPrefabs<T>(bool collectChildTypes) where T : IPoolObject {
        Type t = typeof(T);
        List<T> result = new List<T>();

        if (inst.objPrefabs.ContainsKey(t)) {
            var prefabs = inst.objPrefabs[t];
            foreach (var item in prefabs) {
                result.Add((T)item.Value);
            }
        }

        if (collectChildTypes) {
            foreach (var a in inst.objPrefabs) {
                foreach (var b in a.Value) {
                    if (b.Value is T && !result.Contains((T)b.Value)) {
                        result.Add((T)b.Value);
                    }
                }
            }
        }

        return result;
    }

    public static void AddSpawned(IPoolObject poolObj) {

    }

    public static void Push(IPoolObject obj) {
        inst.Internal_Push(obj);
    }

    public const string DEFAULT_SUB_TYPE = "default";

}

public interface IPoolObject {
    string subType { get; }
    Type GetPoolObjectType();
    GameObject GetGameObject();
    void OnCreate();
    void OnPop();
    void OnPush();
}