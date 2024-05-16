using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    //This game may play with Gravity fields and directions, as such every creature needs to consider the direction of gravity.
    [Header("Gravity")] 
    //This vector stores the current direction of the gravity.
    public Vector2 gravityDirection = new(0,-1);
    public float defaultGravityAcceleration;
    public float currentGravityAcceleration;

    private Vector2 _gravityForce;

    private void Start()
    {
        //Application.targetFrameRate = 144;
        //TODO: GET GRAVITY FROM LEVEL
    }

    private void FixedUpdate()
        {
            //Ensures the creature is correctly rotated according to the current gravity
            float rot = Mathf.Atan2(gravityDirection.y , gravityDirection.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(0 , 0, rot + 90);
            
            //Calculate the Force of gravity on the object
            _gravityForce = currentGravityAcceleration * gameObject.GetComponent<Rigidbody2D>().mass * gravityDirection;
            
            //Accelerates the creature according to the current gravity
            gameObject.GetComponent<Rigidbody2D>().AddForce(_gravityForce);
        }
}
