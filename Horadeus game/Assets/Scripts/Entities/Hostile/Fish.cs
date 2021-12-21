using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fish : Entity {

    #region Variables
    public Rigidbody rb;
    public NavMeshAgent agent;
    public GameObject moveToSpot;
    #endregion
    public override Type GetPoolObjectType() {
        return typeof(Fish);
    }

    #region Built In Methods
    private void Start()
    {
        agent = this.gameObject.GetComponent<NavMeshAgent>();
        moveToSpot = GameObject.Find("MoveToSpot");
        agent.destination = moveToSpot.transform.position;
    }


    private void Update()
    {
        
    }
    #endregion

    #region Custom Methods
    #endregion
}
