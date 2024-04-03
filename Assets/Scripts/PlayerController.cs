using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //This script handles the player movement and other controls

    [Header("Movement")]
    //The maximum speed of Houston
    public float maxSpeed;

    private float _movementForce;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey("right"))
        {
            _rb.AddForce(_movementForce * transform.right);
        }

        if (Input.GetKey("left"))
        {
            _rb.AddForce(_movementForce * -transform.right);
        }
    }

    private void FixedUpdate()
    {
        //We need to calculate the amount of force needed to accelerate Houston to the desired maximum speed
        //F = m * a, a = v0 - vf / t
        _movementForce = _rb.mass * ((maxSpeed - _rb.velocity.x) / (Time.fixedDeltaTime * 1000));
    }
}
