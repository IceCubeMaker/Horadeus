using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI inst;

    public TextMeshProUGUI arrowCountText;
    public Image crosshairImage;
    public Image NoAmoSignImage;
    public Animator crosshairAnimator;

    public GameObject hotbar;
    public GameObject hotbarSlot;

    public void Init() {
        inst = this;
    }

    public void UpdatePlayerInventoryHUD() { //this function gets triggered in a different script
        ItemData arrowItem = Game.inst.player.inventory.GetItem(ItemType.Arrow);
        arrowCountText.text = "Arrows: " + arrowItem.count; //shows the amount of arrows on screen
    }

    
    public void EnableCrosshair(bool enable) { //This is a function gets triggered in a different script
        if (enable) {
            crosshairAnimator.ResetTrigger("StopShooting");
            crosshairAnimator.SetTrigger("StartShooting"); //Starts Animation
        } else {
            crosshairAnimator.ResetTrigger("StartShooting");
            crosshairAnimator.SetTrigger("StopShooting"); //Resets Animation
        }
        crosshairImage.enabled = enable; //Makes crosshair visible
    }
    

    public void EnableNoAmoSign(bool enable) { //This is a function gets triggered in a different script
        NoAmoSignImage.enabled = enable; //Show a red "!" sign on screen when the player is out of amo
    }

    public void UpdateHotbar() // Update Hotbar for player
    {
        if (!hotbar) { return; } //Returns if hotbar doesn't exist

        //Deactivate Hotbar slots
        for(int i = 0; i<hotbar.transform.childCount; ++i)
        {
            hotbar.transform.GetChild(i).gameObject.SetActive(false);
        }

        //Reactivate Hotbar Slots
        for (int i = 0; i < Game.inst.player.playerInventory.items.Count; ++i)
        {
            hotbar.transform.GetChild(i).gameObject.SetActive(true);
            hotbar.transform.GetChild(i).GetComponentInChildren<Image>().sprite = Game.inst.player.playerInventory.items[i].invItem.itemImage;
        }
    }

}
