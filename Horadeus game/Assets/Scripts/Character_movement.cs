using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_movement : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    public float m_Speed = 5f;
    public float faceCamSpeed = 0.1f;

    public Vector2 offsetInPlane;
    public float sensX, sensY;
    public float rotX, rotY;
    public float dst = 10;

    public Transform cam;
    public Camera camera;

    public Animator animator;

    // Returns whether or not the given position is in the game window
    public bool isInWindow(Vector2 position)
    {
        return (position.x >= 0 & position.x <= Screen.width &
                position.y >= 0 & position.y <= Screen.height);
    }

    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; //Player can't tip over
        camera = gameObject.GetComponentInChildren<Camera>();
    }

    private void Update() {
        // Gets the cursor position relative to the game window
        Vector2 cursorPosition = camera.ScreenToViewportPoint(Input.mousePosition);
        // Scales the position properly
        cursorPosition.x *= camera.scaledPixelWidth;
        cursorPosition.y *= camera.scaledPixelHeight;

        // Stops this function from running when the cursor isn't inside the game
        if (!isInWindow(cursorPosition))
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * sensY;

        rotY += mouseX * Time.deltaTime;
        rotX = Mathf.Clamp(rotX + mouseY * Time.deltaTime, -70, 70);

        Quaternion rot = Quaternion.Euler(0, rotY, 0) * Quaternion.Euler(rotX, 0, 0);
        Vector3 camPos = transform.position + rot * Vector3.forward * dst;
        
        cam.rotation = rot;
        cam.forward = -cam.forward;

        camPos += cam.right * offsetInPlane.x + cam.up * offsetInPlane.y;

        cam.position = camPos;
    }

    private void LateUpdate() {
        
    }

    void FixedUpdate()
    {
        // Gets the cursor position relative to the game window
        Vector2 cursorPosition = camera.ScreenToViewportPoint(Input.mousePosition);
        // Scales the position properly
        cursorPosition.x *= camera.scaledPixelWidth;
        cursorPosition.y *= camera.scaledPixelHeight;

        // Stops this function from running when the cursor isn't inside the game
        if (!isInWindow(cursorPosition))
        {
            return;
        }

        //make character smootly face camera
        Vector3 lookPos = (cam.position - transform.position) * -1; //-1 because want the back to face the cam. not the front
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, faceCamSpeed);

        Vector3 cam_orientation = new Vector3(cam.forward.x, 0, cam.forward.z); //get camera orientation
        Vector3 m_Input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); //Store user input as a input vector
        m_Input.Normalize();
        Vector3 m_Movement = cam_orientation * m_Input.z + transform.right * m_Input.x;
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Speed * Time.fixedDeltaTime); //move character


        animator.SetFloat("Foward/Backward", m_Input.z); //if > 0: foward, if < 0: backward
        animator.SetFloat("Left/Right", m_Input.x); //if > 0: right, if < 0: left
    }

}
