using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile {

    // Multiplier for the force of gravity
    // Used for customizing gravity for this object specifically
    private static float g_multiplier = 0.1f;

    protected override void Interact(Collision c) {
        base.Interact(c);

        Fish fish = c.gameObject.GetComponent<Fish>();

        if(fish != null) {
            fish.GetComponent<Rigidbody>().useGravity = true; // Makes the fish fall
        }
    }

    public override Type GetPoolObjectType() {
        return typeof(Arrow);
    }

    private void FixedUpdate()
    {
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        rigidBody.AddForce(Physics.gravity * rigidBody.mass * g_multiplier); // Adds the force of gravity
    }

}
