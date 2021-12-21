using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {

    public SO_PlayerInventory inventory;
    public Weapon currentWeapon;

    public PlayerMovement movement;

    private ItemData arrowItem;

    [HideInInspector] public HCamera playerCamera;

    public void Init(HCamera cam) {
        Debug.Log("Init player");

        playerCamera = cam;

        SwitchCursorLock();

        arrowItem = inventory.GetItem(ItemType.Arrow);

        movement.Init(cam);

        currentWeapon.Equip(this);
    }

    public void InternalUpdate() {
        movement.InternalUpdate();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SwitchCursorLock();
        }

        SomeCursorCheck();

        if (currentWeapon != null) {
            if (arrowItem.count > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentWeapon.UseStart();
                    GameUI.inst.EnableCrosshair(true);
                }
            }

            if (Input.GetMouseButton(0))
            {
                currentWeapon.UseHold();

                playerCamera.SetZoomPercent(currentWeapon.charge / currentWeapon.maxChargeTime);
            }

            if (Input.GetMouseButtonUp(0))
            {
                currentWeapon.UseRelease();
                playerCamera.SetZoomPercent(0f);
                GameUI.inst.EnableCrosshair(false);
                inventory.TakeItem(arrowItem, 1);
            }
        }
    }

    private void SomeCursorCheck()
    {
        // Gets the cursor position relative to the game window
        Vector2 cursorPosition = playerCamera.cameraComponent.ScreenToViewportPoint(Input.mousePosition);
        // Scales the position properly
        cursorPosition.x *= playerCamera.cameraComponent.scaledPixelWidth;
        cursorPosition.y *= playerCamera.cameraComponent.scaledPixelHeight;

        // Makes cursor invisible only when it's within the game window
        //if (movement.isInWindow(cursorPosition))
        //{
        //    Cursor.visible = false;
        //} else
        //{
        //    Cursor.visible = true;
        //} //This dose not help during building up the game it just keeps unlocking the cursor fromt he game.
    }

    public void InternalFixedUpdate()
    {
        movement.InternalFixedUpdate();
    }

    private void SwitchCursorLock() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

}