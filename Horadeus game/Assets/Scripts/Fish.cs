using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fish : Entity{

    #region Variables
    public float moveForce = 10;
    public float findNextSpotDist = 1;
    private int currentTarget = -1;
    public float healvalue = 20;
    public InventoryItem itemdrop;

    private Animator anim;
    public Rigidbody rb;
    public NavMeshAgent agent;
    System.Random rnd = new System.Random();
    [SerializeField] private GameObject[] moveToSpots; //This can be added too or taken away to have more areas to swim
    #endregion

    bool isDead = false;

    public override Type GetPoolObjectType() {
        return typeof(Fish);
    }

    #region Built In Methods
    private void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        moveToSpots = GameObject.FindGameObjectsWithTag("FishNavLocation");//GameObject In Hierarchy
        //agent.SetDestination(moveToSpots[rnd.Next(0,moveToSpots.Length)].transform.position);
        //agent.baseOffset = rnd.Next(1, 10); //Increase or lower to adjust hight fish fly
    }


    //Detects collision with arrow
    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.name == "Arrow(Clone)"){
            Debug.Log("Arrow hit");
            Die(); //Kill fish :c
        }

        if(isDead && collision.gameObject.CompareTag("Player")) //check if fish dead and player hit object
        {

            // Add fish to inventory if Health is higher than heal
            if (Game.inst.player.playerHealth + healvalue > Game.inst.player.playerHealthMax)
            {
                Game.inst.player.playerInventory.AddItem(Instantiate(itemdrop), 1); //adding fish to player inventory
            }
            else
            {
                Game.inst.player.HealPlayer(healvalue); //heal player
            }
            Destroy(this.gameObject); //make fish go byebye
        }
    }
    private void Die() //Set death state
    {
        isDead = true;
        anim.SetBool("isDead", true); //Set death animations
        rb.velocity = Vector3.zero; //Cause it to lose any velocity so it falls down straight
        rb.angularVelocity = Vector3.zero; //Set rotation speed to zero
        rb.useGravity = true; //Allow gravity to affect it
        agent.enabled = false; //Stop NavMeshAgent
    }


    private void Update()
    {
        if (isDead){ return; } //Don't run code if in death state
        FindRandomSpot(); //If Possible find and move to different moveToSpot positions
    }

    private void OnDrawGizmos()//Draws Gizmos if enabled in editor
    {
        Gizmos.DrawWireSphere(transform.position, findNextSpotDist); //Draw a Wireframe Sphere around fish to show findNestSpotDist
    }

    #endregion

    #region Custom Methods
    /// <summary>
    /// This Method will allow the fish to find a random location in the map; They are empty Gameobject array
    /// </summary>
    private void FindRandomSpot()
    {
        if (moveToSpots.Length == 0) { return; } //Exit if no spots

        //Find position if not following one or if distance is lower than findNextSpotDist
        if (currentTarget == -1 || Vector3.Distance(transform.position, moveToSpots[currentTarget].transform.position) <= findNextSpotDist)
        {
            currentTarget = UnityEngine.Random.Range(0, moveToSpots.Length); //Set new target
        }

        //-------------------------------Movement Code--------------------------------------

        //Adds velocity towards the target to move to it
        rb.velocity = ((moveToSpots[currentTarget].transform.position-transform.position).normalized * moveForce * Time.deltaTime);

        //Makes fish face the target slowly
        transform.forward = Vector3.Lerp(transform.forward, rb.velocity.normalized, 0.8f * Time.deltaTime*10).normalized;

        //----------------------------------------------------------------------------------

    }

    #endregion
}
