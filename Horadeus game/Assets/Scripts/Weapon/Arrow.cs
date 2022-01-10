using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*------------------------------------------------------------------
NOTES / TO-DO
There is a script that makes it so that the arrow sticks to the object it hits, however right now, 
the Arrow gets turned invisible directly after hitting an object and also gets destroyed very soon after. 
We want to change this so that the arrow only gets invisible and destroyed if it hits an enemy, 
while it still sticks to objects like trees and terrain. 

-------------------------------------------------------------------*/
public class Arrow : Projectile {

    public AudioSource audioSource; //select a place where the audio is played from
    public AudioClip[] clips; //select and audio source
    public Rigidbody rigidBody; //select a rigidbody
    public MeshRenderer theMeshRenderer; //select a meshrenderer
    public TrailRenderer theTrailRenderer; //select a trail renderer
    

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

    void OnCollisionEnter(Collision other) //if arrow hits an object
    {   
        gameObject.transform.parent = other.transform; //makes the arrow a part of whatever object it hits
        rigidBody.isKinematic = true; 
        rigidBody.velocity = new Vector3(rigidBody.velocity.x , rigidBody.velocity.y , 0);
        rigidBody.detectCollisions = false; //the arrow will stop looking for collisions
        audioSource.PlayOneShot(clips[0]); //play sound effect

        //---------------------------------------------------------------------------
        //this is the part that needs to only run when the arrow hits an enemy
        
        theMeshRenderer.enabled = false; //make the arrow invisible
        theTrailRenderer.enabled = false; //make the trail invisible
        Destroy(gameObject, GetComponent<AudioSource>().clip.length);//destroy arrow after the sound that is being played is finished
        //---------------------------------------------------------------------------
    } 
}
