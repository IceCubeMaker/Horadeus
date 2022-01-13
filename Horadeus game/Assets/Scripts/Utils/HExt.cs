using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HExt
{
    public static Transform GetTopRootTransform(this Transform t)
    {
        Transform root = t;
        
        while(root.parent != null)
        {
            root = root.parent;
        }

        return root;
    }

    public static bool IsMyChildRecursively(this Transform parent, Transform child)
    {
        Transform[] allChilds = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < allChilds.Length; i++)
        {
            if(allChilds[i] == child)
            {
                return true;
            }
        }

        return false;
    }
}
