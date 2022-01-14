using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Horadeus/CameraConfig")]
public class SO_HCameraConfig : ScriptableObject
{
    [Header("General")]
    public Vector2 minMaxZoomFOV;
    public Vector2 defaultOffset;
    public float defaultFollowDst = 10;
    public bool usePositionSmooth = false;
    public float positionSmooth = 0.2f;

    [Header("Input")]
    public float sensX = 100, sensY = 100;

    [Header("Combat")]
    public Vector2 offsetWhenAiming;

    [Header("Anti-Clipping")]
    public LayerMask clipMask;
    public float minObjectBoundsXZToClip = 1f;
    public bool debugDraw = true;
}
