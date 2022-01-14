using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolObject {

    public ProjectileType projectileType;

    public LayerMask interactMask;
    public Rigidbody rb;

    public float additionalGravity = 5f;
    public Vector3 refRotationDireciton;
    public TrailRenderer trailRend;

    private bool doClearTrail;

    public override void OnPop() {
        base.OnPop();

        rb.velocity = Vector3.zero;
        if (trailRend != null) {
            doClearTrail = true;
        }
    }

    public override void OnPush() {
        base.OnPush();

        if (trailRend != null) {
            doClearTrail = true;
        }
    }

    private void Update() {
        if (rb.velocity.magnitude > 5f) {
            rb.MoveRotation(Quaternion.FromToRotation(refRotationDireciton, rb.velocity));
        }
    }

    protected virtual void LateUpdate() {
        if (doClearTrail) {
            trailRend.Clear();
            doClearTrail = false;
        }
    }

    private void FixedUpdate() {
        rb.AddForce(Vector3.up * -additionalGravity * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision) {
        if (HUtils.LayerInMask(interactMask, collision.gameObject.layer)) {
            Interact(collision);
        }
    }

    protected virtual void Interact(Collision collsion) {

    }

    public virtual void Shoot(Vector3 force) {
        rb.AddForce(force);
    }

    public override Type GetPoolObjectType() {
        return typeof(Projectile);
    }
}

public enum ProjectileType {
    None,
    Arrow
}