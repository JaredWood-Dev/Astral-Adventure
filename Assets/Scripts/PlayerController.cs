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
    //The jumping strength of Houston
    public float jumpPower;
    public float reactionPower;

    private float _movementForce;
    private Vector2 _reactiveForce;
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

        if (Input.GetKeyDown("space"))
        {
            _rb.AddForce(transform.up * (jumpPower * 1000));
        }
    }

    private void FixedUpdate()
    {
        //We need to calculate the amount of force needed to accelerate Houston to the desired maximum speed
        //F = m * a, a = v0 - vf / t
        _movementForce = _rb.mass * ((maxSpeed - Mathf.Abs(_rb.velocity.x)) / (Time.fixedDeltaTime * 100));
        print(_movementForce);

        //This calculates the reactive force to slow Houston down for tighter controls.
        _reactiveForce = _rb.velocity.normalized * -1 * reactionPower;
    }
}
