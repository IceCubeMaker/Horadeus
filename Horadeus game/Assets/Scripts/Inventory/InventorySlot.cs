using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    public InventoryItem invItem;
    public int amount = 0;

    public void AddAmount(int toadd)
    {
        amount += toadd;
    }
}
