using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu]
public class SO_PlayerInventory : ScriptableObject
{
    public List<ItemData> items;
    public List<SO_Item> itemsInfo;

    public GameEvent_SO OnInventoryChanged;

    public void OnInventoryRestored()
    {
        OnInventoryChanged.Raise();
    }

    public bool TakeItem(ItemData item, int amount)
    {
        if (item.count >= amount)
        {
            item.count -= amount;
            OnInventoryChanged.Raise();
            return true;
        } else
        {
            return false;
        }
    }

    public ItemData GetItem(ItemType itemType)
    {
        return items.Find(x => x.type == itemType);
    }

}

[System.Serializable]
public class ItemData
{
    public ItemType type;
    public int count;
}

[System.Serializable]
public class GD_PlayerInventory : GD
{
    public List<ItemData> itemsData;

    public GD_PlayerInventory() : base(GDType.Inventory, GDLoadOrder.Pre_0)
    {
        SetDefaults(default);
    }

    public void RestoreInventory(SO_PlayerInventory playerInventory)
    {
        playerInventory.items.Clear();

        for (int i = 0; i < itemsData.Count; i++)
        {
            Debug.Log(itemsData[i].count + " " + itemsData[i].type);
            playerInventory.items.Add(itemsData[i]);
        }
        playerInventory.OnInventoryRestored();
    }

    public GD_PlayerInventory(SerializationInfo info, StreamingContext sc) : base(info, sc)
    {
        itemsData = (List<ItemData>)info.GetValue("items", typeof(List<ItemData>));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        info.AddValue("items", itemsData);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds)
    {
        itemsData = new List<ItemData>();
        itemsData.Add(new ItemData() {
            type = ItemType.Arrow,
            count = 50
        });
    }
}
