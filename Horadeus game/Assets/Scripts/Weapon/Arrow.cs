using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile {

    // Multiplier for the force of gravity
    // Used for customizing gravity for this object specifically
    private static float g_multiplier = 0f;

    public override Type GetPoolObjectType() {
        return typeof(Arrow);
    }

    private void FixedUpdate()
    {
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        rigidBody.AddForce(Physics.gravity * rigidBody.mass * g_multiplier); // Adds the force of gravity
    }

}
