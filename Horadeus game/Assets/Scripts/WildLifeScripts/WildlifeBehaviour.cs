using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildlifeBehaviour : MonoBehaviour
{
    [Header("Stats")]
    public int health; //Set the health stat 
    public int defense; //Set the defense stat
    public int moveSpeed; //Set speed stat
    
    [Header("Chase Target")]
    public bool enableChaseTarget; //enable/disable chasing a target
    public bool chaseWhenNotInRange; //if this is disabled the creature will follow the player no matter where they are
    public Transform Target; //this is the target the player is supposed to follow
    private float dist; //dist = distance ; it is the distance towards the target
    public float sightRadius; //if you enter this radius, the enemy will follow you
    
    [Header("Others")]
    public bool dontTakeDamage; //Makes it impossible for this entity to take any damage
    public bool noDamageFromArrows; //Disables taking damage from arrows
    
    private int damageCounter; //this variable will be used to calculate how much damage the entity takes
    
    
//------------------------------Before first frame-----------------------------------------
    void Start() 
    {
        if (defense <= 0) { //if defense is equel 0 or less
            defense = 1; //default the defense variable to 1, so that it doesn't cause any bugs or complications
        }
        if (health <= 0) { //if health is below 0 or equal
            health = 1; //default the health variable to 1, to prevent any bugs or complications
        }
    }


//----------------------------------Take Damage Script-------------------------------- (BUG PRESENT)
    
    void OnCollisionEnter(Collision collision) { //if an arrow hits the entity
        if (collision.gameObject.tag == "Damage") {
            if (!dontTakeDamage) { //if the entity can take damage
                damageCounter = 0; //default damagecounter to 0
                if (!noDamageFromArrows) { //if damage from arrows is turned on
                    damageCounter = 100; //set damage counter to 100
                }
                health = health - (damageCounter / defense); //put damage
            }
        }
    }

//----------------------------------------Death Script--------------------------------
    void Update() // Update is called once per frame
    {

        if (health <= 0) { //if health is 0 or below 0
            health = 0; //default health to 0 to not cause any bugs
            Destroy(gameObject); //destroy the entity
        }



//----------------------------------Chase Target Script----------------------------------
        if (enableChaseTarget) { 
            dist = Vector3.Distance(Target.position, transform.position);

            if (dist <= sightRadius) {//if the player is in sight radius
                transform.LookAt(Target); //make the object turn towards the target
                GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed); //makes thecreature run after the target
            }
            if (chaseWhenNotInRange) {//if the creature chases the target no matter if it's in sight or not
                transform.LookAt(Target);
                GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed);
            }
        }
//----------------------------------------------------------------------------------------
    }
}
