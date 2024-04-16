using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] 
    public float movementSpeed;
    public float targetMovementSpeed;

    private Rigidbody2D _rb;
    private Creature _c;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _c = GetComponent<Creature>();
    }

    private void FixedUpdate()
    {
        //Need to get the velocity in the direction of gravity
        float v0 = (_rb.velocity.magnitude * transform.right).magnitude;

        print(v0);
        
        //Calculate the force necessary to move the character
        float a = (targetMovementSpeed - v0) / Time.deltaTime;

        //Calculate the force vector
        Vector2 f = a * _rb.mass * transform.right;
        
        //Apply the force to the character
        _rb.AddForce(f);
    }

    private void Update()
    {
        //Change the target movement speed based on the input
        targetMovementSpeed = movementSpeed * Input.GetAxis("Horizontal");
    }

    public void MoveCharacter()
    {
        
    }
}
