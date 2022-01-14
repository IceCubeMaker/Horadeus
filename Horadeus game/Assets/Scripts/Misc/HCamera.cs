using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public Camera cameraComponent;

    public CameraTarget target;

    [Header("Config")]
    public SO_HCameraConfig config;

    private float rotX, rotY;
    private Vector2 currerntOffsetInPlane;
    private Vector2 targetOffsetInPlane;
    private float targetFov;

    private Vector3 targetCamPos;
    private Vector3 posSmoothDamp;

    private RaycastHit[] hitCache = new RaycastHit[10];
    private Transform targetTopRootParent;

    public void Init()
    {
        targetFov = config.minMaxZoomFOV.y;
        targetOffsetInPlane = config.defaultOffset;
        currerntOffsetInPlane = config.defaultOffset;
    }

    public void InternalUpdate()
    {
        cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, targetFov, Time.deltaTime * 10f);
        currerntOffsetInPlane = Vector2.Lerp(currerntOffsetInPlane, targetOffsetInPlane, Time.deltaTime * 10f);

        Look();
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * config.sensX;
        float mouseY = Input.GetAxis("Mouse Y") * config.sensY;

        rotY += mouseX * Time.deltaTime;
        rotX = Mathf.Clamp(rotX + mouseY * Time.deltaTime, -70, 70);

        Quaternion rot = Quaternion.Euler(0, rotY, 0) * Quaternion.Euler(rotX, 0, 0);
        Vector3 camPos = target.transform.position + rot * Vector3.forward * config.defaultFollowDst;

        cameraTransform.rotation = rot;
        cameraTransform.forward = -cameraTransform.forward;

        camPos += cameraTransform.right * currerntOffsetInPlane.x + cameraTransform.up * currerntOffsetInPlane.y;

        // Anti-clipping
        Vector3 clipedCamPos = camPos;

        Vector3 dd = (camPos - target.transform.position);
        Vector3 clipDir = dd.normalized;
        Ray targetToPotentialCamPos = new Ray(target.transform.position, clipDir);
        float minClipedDst = dd.magnitude;

        int hitCount = Physics.RaycastNonAlloc(targetToPotentialCamPos, hitCache, config.defaultFollowDst, config.clipMask, QueryTriggerInteraction.Ignore);

        if(hitCount > 0)
        {
            for (int i = 0; i < Mathf.Min(hitCount, hitCache.Length); i++)
            {
                Bounds colliderBounds = hitCache[i].collider.bounds;

                if (colliderBounds.size.x < config.minObjectBoundsXZToClip || colliderBounds.size.z < config.minObjectBoundsXZToClip)
                {
                    if (config.debugDraw)
                    {
                        IMDraw.Bounds(colliderBounds, Color.green);
                    }
                    continue;
                }

                if (config.debugDraw) { 
                    IMDraw.Bounds(colliderBounds, Color.red);
                }

                Vector3 clipedPos = hitCache[i].point - clipDir * 0.5f;
                float newDst = (clipedPos - targetToPotentialCamPos.origin).magnitude;

                if (newDst < minClipedDst)
                {
                    clipedCamPos = clipedPos;
                    minClipedDst = newDst;
                }

                
                Transform hitRootT = hitCache[i].transform.GetTopRootTransform();
                if(hitRootT != targetTopRootParent)
                {
                    
                }
            }
        }

        camPos = clipedCamPos;
        // Anti-clipping

        targetCamPos = camPos;

        if (config.usePositionSmooth)
        {
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetCamPos,ref posSmoothDamp, config.positionSmooth);
        } else
        {            
            cameraTransform.position = targetCamPos;
        }
    }

    public void SetTarget(CameraTarget _target)
    {
        target = _target;

        targetTopRootParent = target.transform.GetTopRootTransform();
    }

    public void SetZoomPercent(float p, bool instant = false)
    {
        targetFov = Mathf.Lerp(config.minMaxZoomFOV.x, config.minMaxZoomFOV.y, 1 - p);
        targetOffsetInPlane = Vector2.Lerp(config.defaultOffset, config.offsetWhenAiming, p);

        if (instant) {
            cameraComponent.fieldOfView = targetFov;
        }
    }

}

[System.Serializable]
public class CameraTarget
{
    public Transform transform;
}
