using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    public Transform shootPoint;
    public Vector2 minMaxShootForce = new Vector2(300, 2000);

    public override void Attack()
    {
        base.Attack();

        ShootArrow();
    }

    private void ShootArrow()
    {
        // Coordinates for the center of the game window
        float mid_x = Screen.width / 2;
        float mid_y = Screen.height / 2;

        //Makes a Ray pointing out towards the middle of the screen
        Ray ray = player.playerCamera.cameraComponent.ScreenPointToRay(new Vector3(mid_x, mid_y, 0));
        RaycastHit hit;
        Vector3 destination;

        // Detects if the ray hits an object, then sets where the arrow should hit
        if (Physics.Raycast(ray, out hit))
        {
            destination = hit.point;
            if (Vector3.Distance(destination, transform.position) < 2) { //if the object is directly in fron of the camera don't shoot it
                destination = player.playerCamera.transform.position + ray.direction * 100;
            }
        } else //If there is no object to hit, the arrow is shot towards a point in the air
        {
            destination = player.playerCamera.transform.position + ray.direction * 100;
        }

        // Fires the arrow towards the destination
        Vector3 shootDirection = destination - shootPoint.position;
        shootDirection.Normalize();

        Arrow arrow = MPool.Get<Arrow>();
        arrow.transform.position = shootPoint.position;
        arrow.transform.forward = shootDirection;
        arrow.Shoot(arrow.transform.forward * Mathf.Lerp(minMaxShootForce.x, minMaxShootForce.y, charge/maxChargeTime));
    }

}
