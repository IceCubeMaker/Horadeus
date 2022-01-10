using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
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
    public bool healthbarVisible; //Enable or disable healthbar

    private int damageCounter; //this variable will be used to calculate how much damage the entity takes


    
    void Start() // Start is called before the first frame update
    {
        if (defense <= 0) { //if defense is equel 0 or less
            defense = 1; //default the defense variable to 1, so that it doesn't cause any bugs
        }
        if (health <= 0) { //if health is below 0 or equal
            health = 1; //default the health variable to 1, to prevent any bugs or complications
        }
    }

    void Update() // Update is called once per frame
    {
        //----------------------------------Take Damage Script--------------------------------
        void OnCollisionEnter(Collision Arrow) { //if an arrow hits the entity
            if (!dontTakeDamage) { //if the entity can take damage
                damageCounter = 0; //default damagecounter to 0
                if (!noDamageFromArrows) { //if damage from arrows is turned on
                    damageCounter = GetComponent<Arrow>().damage; //set damage counter to the damage value of the arrow
                }
                health = health - (damageCounter / defense);
            }
        }
        //----------------------------------------Death Script--------------------------------
        if (health <= 0) { //if health is 0 or below 0
            health = 0; //default health to 0 to not cause any bugs
            Destroy(gameObject); //destroy the entity
        }

        //----------------------------------Chase Target Script----------------------------------
        if (enableChaseTarget) { 
            dist = Vector3.Distance(Target.position, transform.position);

            if (dist <= sightRadius) {
                transform.LookAt(Target);
                GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed);
            }
            if (chaseWhenNotInRange) {
                transform.LookAt(Target);
                GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed);
            }
        } 
        //----------------------------------Health Bar Script------------------------------------
        if (healthbarVisible) {
            //Add a script to create a healthbar that floats above the entity here

        }
        //---------------------------------------------------------------------------------------
    }
}
