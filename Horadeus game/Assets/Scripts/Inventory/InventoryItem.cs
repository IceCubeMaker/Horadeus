using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu()]
public class InventoryItem : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite itemImage;
    public UnityEvent itemConsumeToDo;

    public virtual void Consume()
    {
        itemConsumeToDo.Invoke();
    }
}
