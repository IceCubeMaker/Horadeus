using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public Camera cameraComponent;

    public Transform target;

    [Header("Config")]
    public Vector2 minMaxZoomFOV;

    private float targetFov;

    public Vector2 offsetDefault;
    public Vector2 offsetWhenAiming;
    public float sensX = 100, sensY = 100;
    private float rotX, rotY;
    public float dst = 10;

    private Vector2 currerntOffsetInPlane;
    private Vector2 targetOffsetInPlane;

    public void Init()
    {
        targetFov = minMaxZoomFOV.y;
        targetOffsetInPlane = offsetDefault;
        currerntOffsetInPlane = offsetDefault;
    }

    public void InternalUpdate()
    {
        cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, targetFov, Time.deltaTime * 10f);
        currerntOffsetInPlane = Vector2.Lerp(currerntOffsetInPlane, targetOffsetInPlane, Time.deltaTime * 10f);

        Look();
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * sensY;

        rotY += mouseX * Time.deltaTime;
        rotX = Mathf.Clamp(rotX + mouseY * Time.deltaTime, -70, 70);

        Quaternion rot = Quaternion.Euler(0, rotY, 0) * Quaternion.Euler(rotX, 0, 0);
        Vector3 camPos = target.position + rot * Vector3.forward * dst;

        cameraTransform.rotation = rot;
        cameraTransform.forward = -cameraTransform.forward;

        camPos += cameraTransform.right * currerntOffsetInPlane.x + cameraTransform.up * currerntOffsetInPlane.y;

        cameraTransform.position = camPos;
    }

    public void SetZoomPercent(float p, bool instant = false)
    {
        targetFov = Mathf.Lerp(minMaxZoomFOV.x, minMaxZoomFOV.y, 1 - p);
        targetOffsetInPlane = Vector2.Lerp(offsetDefault, offsetWhenAiming, p);

        if (instant) {
            cameraComponent.fieldOfView = targetFov;
        }
    }

}
