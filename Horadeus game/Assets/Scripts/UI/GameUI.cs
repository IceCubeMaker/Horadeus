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

    public void Init() {
        inst = this;
    }

    public void UpdatePlayerInventoryHUD()
    {
        ItemData arrowItem = Game.inst.player.inventory.GetItem(ItemType.Arrow);
        arrowCountText.text = "Arrows: " + arrowItem.count;
    }

    
    public void EnableCrosshair(bool enable)
    {
        crosshairImage.enabled = enable;
    }
    

    public void EnableNoAmoSign(bool enable)
    {
        NoAmoSignImage.enabled = enable;
    }

}
