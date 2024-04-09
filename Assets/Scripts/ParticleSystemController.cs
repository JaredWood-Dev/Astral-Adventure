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

    private void Start()
    {
        //Create the particle system
        targetSystem = Instantiate(targetSystem);
    }

    private void Update()
    {
        //Keep the particle system with the parent object
        targetSystem.transform.position = gameObject.transform.position + systemOffset;
    }

    public void DeleteSystem()
    {
        Destroy(targetSystem);
    }

    public void StartSystem()
    {
        targetSystem.Play();
        print(targetSystem);
    }

    public void StopSystem()
    {
        targetSystem.Stop();
    }
}
