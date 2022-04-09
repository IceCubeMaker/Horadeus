using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent interactActions;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        Game.inst.toInteract = this;
        GameUI.inst.CanInteract(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }
        if (Game.inst.toInteract == this)
        {
            Game.inst.toInteract = null;
            GameUI.inst.CanInteract(false);
        }
    }
}
