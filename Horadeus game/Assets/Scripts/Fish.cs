using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fish : Entity{

    #region Variables
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
        agent.SetDestination(moveToSpots[rnd.Next(0,moveToSpots.Length)].transform.position);
        agent.baseOffset = rnd.Next(1, 10); //Increase or lower to adjust hight fish fly
    }


    //Detects collision with arrow
    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.name == "Arrow(Clone)"){
            Debug.Log("Arrow hit");
            Die(); //Kill fish :c
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
        if (isDead == false) 
        {
            FindRandomSpot();
        }
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// This Method will allow the fish to find a random location in the map; They are empty Gameobject array
    /// </summary>
    private void FindRandomSpot()
    {
        if (agent.remainingDistance < 1)
        {
            if (moveToSpots != null)
            {
                agent.SetDestination(moveToSpots[rnd.Next(0, moveToSpots.Length)].transform.position);
            }
        }

    }



    #endregion
}
