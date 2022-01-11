using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BillboardEffect : MonoBehaviour
{

    public bool ShouldRevertSprite = true;
    public float faceCamSpeed = 0.1f;
    
    public Transform cam;
    private Rigidbody m_Rigidbody;
    public Animator animator;
    private SpriteRenderer spriteRenderer;


    float yStartRotation;
    int FrontBackLeftRight = 1;

    Vector3 Scale;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        //get starting y rotation value
        yStartRotation = transform.rotation.eulerAngles.y;
        Debug.Log("Start y rotation: " + (yStartRotation));
        //Debug.Log("Start y rotation: " + (transform.eulerAngles(yStartRotation)));

        Scale = transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        //always look at cam
        Vector3 lookPos = (cam.position - transform.position)*-1;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, faceCamSpeed);
        //get current orientation (in degrees)
        float yCurrentRotation = transform.rotation.eulerAngles.y;

        //get rotation if character wasn't rotating towards camera
        float yImaginaryRotation = (yCurrentRotation - yStartRotation)%360; //make sure it's between -360 and 360
        if (yImaginaryRotation < 0) { yImaginaryRotation += 360; } //make sure it's between 0 and 360

        //will revert sprite depending on the angle
        if ((ShouldRevertSprite == true) & (yImaginaryRotation > 180))
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        //Animator
        if (((yImaginaryRotation < 45) & (yImaginaryRotation > 0)) | ((yImaginaryRotation > 315) & (yImaginaryRotation < 360)))
        {
            FrontBackLeftRight = 1;
        }
        else if ((yImaginaryRotation < 135) & (yImaginaryRotation > 45))
        {
            FrontBackLeftRight = 3;
        }
        else if ((yImaginaryRotation < 225) & (yImaginaryRotation > 135))
        {
            FrontBackLeftRight = 2;
        }
        else if ((yImaginaryRotation < 315) & (yImaginaryRotation > 225))
        {
            FrontBackLeftRight = 3;
        }

        animator.SetInteger("Front,Back,Left,Right", FrontBackLeftRight);
    }
}
