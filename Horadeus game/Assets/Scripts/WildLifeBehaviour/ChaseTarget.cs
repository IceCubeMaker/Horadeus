using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTarget : MonoBehaviour
{
    public Transform Target;
    private float dist; //dist = distance
    public float moveSpeed;
    public float sightRadius;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(Target.position, transform.position);

        if (dist <= sightRadius) {
            transform.LookAt(Target);
            GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed);
        }
    }
}
