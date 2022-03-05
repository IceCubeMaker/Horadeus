using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    public RectTransform healthbar; //Use to change the x scale to impliment healthbar
    private WildlifeBehaviour parentwlb; //Use for parent WildlifeBehaviour script

    // Start is called before the first frame update
    void Start()
    {
        //----------------------- Setting Variables -------------------------
        
        GetComponent<Canvas>().worldCamera = Camera.main;
        parentwlb = GetComponentInParent<WildlifeBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.forward; //Make Canvas face same direction as camera

        if (healthbar) //Set healthbar scale
        {
            healthbar.localScale = new Vector3((float)parentwlb.health / parentwlb.totalHealth, 1f, 1f);
        }
    }
}
