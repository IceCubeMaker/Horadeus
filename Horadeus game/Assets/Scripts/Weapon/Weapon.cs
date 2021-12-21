using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isChargable;
    public WeaponState state;
    public float charge;
    public float maxChargeTime = 2f;

    protected Player player;

    public void Equip(Player player)
    {
        this.player = player;
    }

    public virtual void UseStart()
    {
        if (state == WeaponState.None)
        {
            if (isChargable)
            {
                state = WeaponState.IsCharging;
            } else
            {
                state = WeaponState.IsAttacking;
            }
        }
    }

    public virtual void UseHold()
    {
        if(state == WeaponState.IsCharging)
        {
            charge += Time.deltaTime / maxChargeTime;
        }
    }

    public virtual void UseRelease()
    {
        if(state == WeaponState.IsCharging)
        {
            Attack();
        }
    }

    public virtual void Attack()
    {
        state = WeaponState.None;
    }

}

public enum WeaponState
{
    None,
    IsCharging,
    IsAttacking
}
