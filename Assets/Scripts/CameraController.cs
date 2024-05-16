using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //This script controlls the camera
    //For now, it is a simple camera that follows Houston
    //TODO: Improve camera system

    public GameObject followTarget;
    public Vector3 cameraOffset;

    private void Update()
    {
        gameObject.transform.position = followTarget.transform.position + cameraOffset;
    }
}
