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

    private float rotX, rotY; //X,Y Variables for Camera rotation
    private Vector2 currerntOffsetInPlane;
    private Vector2 targetOffsetInPlane;
    private float targetFov; //Change Fov to this value every update

    private Vector3 targetCamPos;
    private Vector3 posSmoothDamp;

    private RaycastHit[] hitCache = new RaycastHit[10];
    private Transform targetTopRootParent;

    public void Init() //Called at start by Game.cs
    {
        // -----------Fetching variables from config scriptable object---------------------

        targetFov = config.minMaxZoomFOV.y; //Fetch max zoom from y component, x is min
        targetOffsetInPlane = config.defaultOffset; //Fetch default offset to target offset
        currerntOffsetInPlane = config.defaultOffset; //Setting value of current to use in transitions to reach the target

        // --------------------------------------------------------------------------------
    }

    public void InternalUpdate() //Called every frame by Game.cs
    {
        cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, targetFov, Time.deltaTime * 10f); //Set fov to target-fov through a Linear Interpolation
        currerntOffsetInPlane = Vector2.Lerp(currerntOffsetInPlane, targetOffsetInPlane, Time.deltaTime * 10f); //Set the offset to target-offset through a Linear Interpolation

        Look(); //Execute camera code
    }

    private void Look()
    {
        //Fetching change in X and Y of Mouse/ Right analog stick for controller and multiplying with sensitivity
        float mouseX = Input.GetAxis("Mouse X") * config.sensX;
        float mouseY = Input.GetAxis("Mouse Y") * config.sensY;

        //-------------------------------------------------------Camera Rotation-----------------------------------------------------------------

        rotY += mouseX * Time.deltaTime; //Adding change in Horizontal value
        rotX = Mathf.Clamp(rotX + mouseY * Time.deltaTime, -70, 70); //Adding change in Vertical value and clamping rotation
        Quaternion rot = Quaternion.Euler(0, rotY, 0) * Quaternion.Euler(rotX, 0, 0); // Combining both rotations into one Quaternion to set
        cameraTransform.rotation = rot; // Setting Camera rotation

        //--------------------------------------------------------Add Offset---------------------------------------------------------------------

        Vector3 camPos = target.transform.position + rot * Vector3.forward * config.defaultFollowDst; //Moves the camera back
        cameraTransform.forward = -cameraTransform.forward; //Makes the camera face inwards to character

        camPos += cameraTransform.right * currerntOffsetInPlane.x + cameraTransform.up * currerntOffsetInPlane.y; //Apply offset to camera

        //-------------------------------------------------------Anti-clipping-------------------------------------------------------------------

        Vector3 clipedCamPos = camPos; //Setting default value of clipped pos, Will be overwritten if needs to be clipped

        Vector3 dd = (camPos - target.transform.position); //Calculate vector from camPos to target.gransform.position
        Vector3 clipDir = dd.normalized; //Normalize to get direction with a length of 1
        Ray targetToPotentialCamPos = new Ray(target.transform.position, clipDir); //Create a Ray Struct to use in Raycast
        float minClipedDst = dd.magnitude; //Use distance from camera to target as min distance clipped

        int hitCount = Physics.RaycastNonAlloc(targetToPotentialCamPos, hitCache, config.defaultFollowDst, config.clipMask, QueryTriggerInteraction.Ignore); //Does a raycast. Returns hits.

        if(hitCount > 0) //If Hit check closest point to target and set camera position
        {
            for (int i = 0; i < Mathf.Min(hitCount, hitCache.Length); i++) // Loop through number of hits to find closest point to target
            {
                //--------------------------------Debugging Collider Bounds of Object hit-----------------------------------

                Bounds colliderBounds = hitCache[i].collider.bounds; //Gets Bounds Struct

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

                //-------------------------------Calculating Closest point to target----------------------------------------

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

        camPos = clipedCamPos; //Setting camPos to be new closest point after clipping

        //-------------------------------------- Anti-clipping END --------------------------------------------------------

        targetCamPos = camPos; //Setting targetCamPos to use for interpolation if needed

        if (config.usePositionSmooth) //Checks if smoothing should be applied
        {
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetCamPos,ref posSmoothDamp, config.positionSmooth); //Sets new position with smoothing
        } else
        {
            cameraTransform.position = targetCamPos; //Directly sets new position
        }
    }

    /// <summary>
    /// Sets new target for camera to follow
    /// </summary>
    /// <param name="_target">new target</param>
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
