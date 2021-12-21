using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public enum ItemType
{
    None,
    Arrow
}

public enum ItemCategory
{
    None,
    Weapon,
    Ammunition,
    Food,
    Equipment
}

[CreateAssetMenu]
public class SO_Item : ScriptableObject
{
    public ItemCategory category;
    public ItemType type;
    public int count;
    [TextArea(2,5)]
    public string description;
    public GameObject prefab;

}

public class GD_InventoryItem : GD
{
    public ItemType itemType;
    public int count;

    public GD_InventoryItem() : base(GDType.InventoryItem, GDLoadOrder.Pre_0)
    {
        SetDefaults(default);
    }

    public void RestoreInventory(SO_PlayerInventory playerInventory)
    {

    }

    public GD_InventoryItem(SerializationInfo info, StreamingContext sc) : base(info, sc)
    {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds)
    {

    }
}
