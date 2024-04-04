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
    public bool onGround;

    private float _movementForce;
    private Vector2 _reactiveForce;
    private Rigidbody2D _rb;
    private Animator _an;
    private Collider2D _foot;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _an = gameObject.GetComponent<Animator>();
        _foot = gameObject.GetComponents<Collider2D>()[1];

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

        if (Input.GetKeyDown("space") && onGround)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
        }
    }

    private void FixedUpdate()
    {
        //We need to calculate the amount of force needed to accelerate Houston to the desired maximum speed
        //F = m * a, a = v0 - vf / t
        if (onGround)
        {
            _movementForce = _rb.mass * ((maxSpeed - Mathf.Abs(_rb.velocity.x)) / (Time.fixedDeltaTime * 100));
        }
        else
        {
            _movementForce = 0.5f * _rb.mass * ((maxSpeed - Mathf.Abs(_rb.velocity.x)) / (Time.fixedDeltaTime * 100));
        }

        //This calculates the reactive force to slow Houston down for tighter controls.
        _reactiveForce = _rb.velocity.normalized * -1 * reactionPower;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && other.otherCollider == _foot)
        {
            onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && other.otherCollider == _foot)
        {
            onGround = false;
        }
    }
}
