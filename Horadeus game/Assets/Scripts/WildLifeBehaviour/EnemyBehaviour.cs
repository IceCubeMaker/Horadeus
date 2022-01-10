using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Stats")]
    /*public int maxHealth; //Set the health stat 
    public int defense; //Set the defense stat*/
    public int moveSpeed; //Set speed stat
    /*public bool damageFromArrows; //Enable or disable getting damage from arrows
    public bool healthbarVisible; //Enable or disable healthbar*/
 
    [Header("Chase Target")]
    public bool enableChaseTarget; //enable/disable chasing a target
    public Transform Target;
    private float dist; //dist = distance
    public float sightRadius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //--------------------------------Chase Target Script-------------------------------
        if (enableChaseTarget) {
            dist = Vector3.Distance(Target.position, transform.position);

            if (dist <= sightRadius) {
                transform.LookAt(Target);
                GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed);
        }
        } 
        //--------------------------------------------------------------------------------
    }
}
