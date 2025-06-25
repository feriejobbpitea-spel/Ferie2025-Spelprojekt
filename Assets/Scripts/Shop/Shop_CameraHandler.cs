using System;
using UnityEngine;

[Serializable]
public class Shop_CameraHandler
{
    public Transform CameraTarget;
    public float Zoom = 2;
    public void MoveCameraToShop()
    {
        CameraFollow.Instance.OverrideTarget = CameraTarget;
        CameraFollow.Instance.OverrideZoom = Zoom;
    }

    public void MoveCameraAwayFromShop()
    {
        CameraFollow.Instance.OverrideTarget = null;
        CameraFollow.Instance.OverrideZoom = -1;
    }
}
