using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    //The particle system
    public ParticleSystem targetSystem;

    //Offset of the system
    public Vector3 systemOffset;

    //Toggle whether the system continues to follow the player
    public bool followObject;

    private void Start()
    {
        //Create the particle system
        targetSystem = Instantiate(targetSystem);
    }

    private void Update()
    {
        //Keep the particle system with the parent object
        if (followObject)
        {
            targetSystem.transform.position = gameObject.transform.position + systemOffset;
            targetSystem.transform.rotation = gameObject.transform.rotation;
        }
    }

    public void DeleteSystem()
    {
        Destroy(targetSystem);
    }

    public void StartSystem()
    {
        targetSystem.transform.position = gameObject.transform.position + systemOffset;
        targetSystem.transform.rotation = gameObject.transform.rotation;
        targetSystem.Play();
    }

    public void StopSystem()
    {
        targetSystem.Stop();
    }
}
