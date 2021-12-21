using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GD_ScriptableObject : MySerializable_SO
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

    public GD_ScriptableObject(GDType _type, GDLoadOrder _sortingOrder)
    {
        gdType = _type;
        gdSortingOrder = _sortingOrder;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("gdTypeByte", gdType);
        info.AddValue("gdSortByte", gdSortingOrder);
    }

    public GD_ScriptableObject(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        foreach (var entry in info)
        {
            switch (entry.Name)
            {
                case "gdTypeByte":
                    _gdType = (byte)(entry.Value);
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
