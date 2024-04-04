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
    private Animator _an;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _an = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey("right"))
        {
            _rb.AddForce(_movementForce * transform.right);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            _an.SetBool("directionPressed", true);
        }
        else if (Input.GetKey("left"))
        {
            _rb.AddForce(_movementForce * -transform.right);
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
            _an.SetBool("directionPressed", true);
        }
        else
        {
            _an.SetBool("directionPressed", false);
        }
    }

    private void FixedUpdate()
    {
        //We need to calculate the amount of force needed to accelerate Houston to the desired maximum speed
        //F = m * a, a = v0 - vf / t
        _movementForce = _rb.mass * ((maxSpeed - _rb.velocity.x) / (Time.fixedDeltaTime * 1000));
    }
}
