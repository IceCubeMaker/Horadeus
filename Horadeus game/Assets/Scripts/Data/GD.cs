using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD : MySerializable
{

    private byte _gdType;
    public GDType gdType
    {
        get {
            return (GDType)_gdType;
        }
        set {
            _gdType = (byte)value;
        }
    }

    private byte _gdSortingOrder;
    public GDLoadOrder gdSortingOrder
    {
        get {
            return (GDLoadOrder)_gdSortingOrder;
        }
        set {
            _gdSortingOrder = (byte)value;
        }
    }

    public GD(GDType _type, GDLoadOrder _sortingOrder)
    {
        gdType = _type;
        gdSortingOrder = _sortingOrder;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("gdTypeByte", gdType);
        info.AddValue("gdSortByte", gdSortingOrder);
    }

    public GD(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        foreach (var entry in info)
        {
            switch (entry.Name)
            {
                case "gdTypeByte":
                    _gdType = (byte)(entry.Value);
                    Debug.Log(gdType);
                    break;
                case "gdSortByte":
                    _gdSortingOrder = (byte)(entry.Value);
                    break;
            }
        }
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds)
    {
        gdType = GDType.None;
        gdSortingOrder = GDLoadOrder.Default;
    }

}

public enum GDLoadOrder : byte
{
    Pre_0,
    Pre_1,
    Default,
    Post_0,
    Post_1
}

public enum GDType : byte
{
    None,
    Game,
    UIWindowData,
    Inventory,
    InventoryItem
}

[Serializable]
public class MySerializable : ISerializable
{

    public MySerializable()
    {

    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {

    }

    public MySerializable(SerializationInfo info, StreamingContext sc)
    {

    }

    [OnDeserializing]
    protected virtual void SetDefaults(StreamingContext ds)
    {

    }

}

[Serializable]
public class MySerializable_SO : ScriptableObject, ISerializable
{

    public MySerializable_SO()
    {

    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {

    }

    public MySerializable_SO(SerializationInfo info, StreamingContext sc)
    {

    }

    [OnDeserializing]
    protected virtual void SetDefaults(StreamingContext ds)
    {

    }

}
