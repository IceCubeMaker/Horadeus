using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    public RectTransform healthbar; //Use to change the x scale to impliment healthbar
    private WildlifeBehaviour parentwlb; //Use for parent WildlifeBehaviour script
    private Canvas canva;

    // Start is called before the first frame update
    void Start()
    {
        //----------------------- Setting Variables -------------------------
        
        GetComponent<Canvas>().worldCamera = Camera.main;
        parentwlb = GetComponentInParent<WildlifeBehaviour>();
        canva = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.forward; //Make Canvas face same direction as camera
        
        //Remove healthbar if full health
        if (parentwlb.health >= parentwlb.totalHealth)
        {
            canva.enabled = false;
            return;
        }

        if (healthbar) //Set healthbar scale
        {
            canva.enabled = true;
            healthbar.localScale = new Vector3((float)parentwlb.health / parentwlb.totalHealth, 1f, 1f);
        }
    }
}
