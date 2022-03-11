using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory")]
public class PlayerInventory : ScriptableObject
{

    #region Variables
    
    public List<InventorySlot> items = new List<InventorySlot>(); //Inventory list
    public Dictionary<string, InventorySlot> itemfinder = new Dictionary<string, InventorySlot>(); //Saves Item ID to allow for faster search in inventory by remembering ID

    private int maxSize = 8;

    #endregion


    //-------------------------------- Inventory Interface --------------------------------

    public void AddItem(InventoryItem id, int amount) //Adds/Remove Item into inventory
    {
        Debug.Log(id);
        if (items.Count >= 8) { return; } //Check if there is space to add

        // Check if Item exists and adds/removes ammount
        if (itemfinder.TryGetValue(id.itemID, out InventorySlot item))
        {
            item.amount += amount; //Adds/Removes item

            // Check If item should be removed
            if (item.amount<= 0)
            {
                items.Remove(item); //Removes item from inventory
                itemfinder.Remove(id.itemID);
            }
        }
        else
        {
            InventorySlot newslot = new InventorySlot() { invItem = id };
            items.Add(newslot); //Adds item to list
            itemfinder[id.itemID] = newslot; //Sets itemfinder value
            newslot.amount = amount;
        }

        GameUI.inst.UpdateHotbar();

    }

}
