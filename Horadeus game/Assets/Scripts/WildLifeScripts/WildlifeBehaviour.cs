using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildlifeBehaviour : MonoBehaviour
{
//---------------------------------------Input---------------------------------------------
    [Header("Stats")]
    public int health; //Set the health stat 
    public int totalHealth;
    public int defense; //Set the defense stat
    public int attackDamage;
    public int damageFromArrows; //this should later be changed, so that it automatically takes the damage that is assigned to the arrow it gets hit by
    public int moveSpeed; //Set speed stat

    [Header("Chase Target")]
    public bool attackPlayer; //should entity attack player
    public bool alwaysChaseTarget; //enable/disable chasing a target
    public bool chaseWhenAttacked; //if this is enabled the snowman will start following the player once they attack them
    public bool chaseWhenInRange; //if this is disabled the creature will follow the player no matter where they are
    public Transform Target; //this is the target the player is supposed to follow
    private float dist; //dist = distance ; it is the distance towards the target
    public float sightRadius; //if you enter this radius, the enemy will follow you

    [Header("Others")]
    public Animator animator; //here you put in the animation of the object this script is attached to
    public bool dontTakeDamage; //Makes it impossible for this entity to take any damage

//---------------------------------private values------------------------------------------
    private int damageCounter; //this variable will be used to calculate how much damage the entity takes
    
//--------------------------------Before first frame---------------------------------------
    void Start() 
    {
        if (defense <= 0) { //if defense is equel 0 or less
            defense = 1; //default the defense variable to 1, so that it doesn't cause any bugs or complications
        }
        if (health <= 0) { //if health is below 0 or equal
            health = 1; //default the health variable to 1, to prevent any bugs or complications
        }
    }


//----------------------------------Take Damage Script/Attack Player----------------------------
    
    void OnCollisionEnter(Collision collision) { //if an arrow hits the entity
        if (collision.gameObject.CompareTag("Damager")) { //check if the collision object has the tag "damager"
            if (!dontTakeDamage) { //if the entity can take damage
                    damageCounter = damageFromArrows; //replace this with the damage that is assigned to the arrow itself
                    if (chaseWhenAttacked) { //if this is enabled the entity will now start chasing the target
                    alwaysChaseTarget = true;
                }
            }
            health = health - (damageCounter / defense); //put damage
        }

//---------------------------------------- Attack Player -----------------------------

        if (attackPlayer==true && collision.gameObject.CompareTag("Player")) //check if collision object is player
        {
            Game.inst.player.HurtPlayer(attackDamage); //damage the player
        }
   }

//-----------------------------------Death Script------------------------------------------
    void Update() // Update is called once per frame
    {

        if (health <= 0) { //if health is 0 or below 0
            health = 0; //default health to 0 to not cause any bugs
            Destroy(gameObject); //destroy the entity
        }



//----------------------------------Chase Target Script------------------------------------
            dist = Vector3.Distance(Target.position, transform.position);

            if (chaseWhenInRange) {
                if (dist <= sightRadius) {//if the target is in sight radius
                    chaseTarget();
                } else {
                    if (!alwaysChaseTarget) {
                        animator.SetBool("Walking", false); //turn off walking animation
                        animator.SetBool("Standing", true); //turn on standing animation
                    }
                }
            }
            if (alwaysChaseTarget) {//if the creature chases the target no matter if it's in sight or not
                chaseTarget();
            }
    }
    void chaseTarget() {
        GetComponent<Rigidbody>().AddForce(-(transform.forward * moveSpeed)); //makes the creature run after the target
        animator.SetBool("Walking", true); //turn on walking animation
        animator.SetBool("Standing", false); //turn off standing animation
    }
}
