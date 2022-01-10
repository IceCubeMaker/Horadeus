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

}
