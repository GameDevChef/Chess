using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    public void SetupCamera(TeamColor team)
    {
        if(team == TeamColor.Black)
        {
            FlipCamera();
        }
    }

    private void FlipCamera()
    {
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -mainCamera.transform.position.z);
        mainCamera.transform.Rotate(Vector3.up, 180f, Space.World);
    }
}
