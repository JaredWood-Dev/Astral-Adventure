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

    [Header("Arcade Gameplay")] 
    public Vector2 momentumBuffer;
    public float jumpBufferTimer = 0.0f;
    public float jumpBufferLimit = 0.1f;
    public bool jumpBuffered;
    public float coyoteTimer = 0.0f;
    public float coyoteLimit = 0.1f;

    private float _movementForce;
    private Vector2 _reactiveForce;
    private Rigidbody2D _rb;
    private Animator _an;
    private Collider2D _foot;
    private Creature _c;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _an = gameObject.GetComponent<Animator>();
        _foot = gameObject.GetComponents<Collider2D>()[1];
        _c = gameObject.GetComponent<Creature>();

    }

    void Update()
    {
        //When pressing a directional key, add the force, scale the appreciate direction, and update the animations
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

        //Attempting to jump, check if on ground, if not buffer the jump
        //Or if we are not on the ground jump if we recently left the ground
        if (Input.GetKeyDown("space") && (onGround || (coyoteTimer < coyoteLimit)))
        {
            Jump();
        }
        else if (Input.GetKeyDown("space"))
        {
            jumpBuffered = true;
            jumpBufferTimer = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        //We need to calculate the amount of force needed to accelerate Houston to the desired maximum speed
        //F = m * a, a = v0 - vf / t
        if (onGround)
        {
            //TODO: REPLACE VX WITH A RELATIVE VECTOR BASED ON GRAVITY DIRECTION
            _movementForce = _rb.mass * (Mathf.Abs(maxSpeed - Mathf.Abs(_rb.velocity.x)) / (Time.fixedDeltaTime * 100));
        }
        else
        {
            _movementForce = 0.5f * _rb.mass * (Mathf.Abs(maxSpeed - Mathf.Abs(_rb.velocity.x)) / (Time.fixedDeltaTime * 100));
        }
        
        //Clamp just in case, no absurd forces silly physics
        _movementForce = Mathf.Clamp(_movementForce, -1500, 1500);

        //This calculates the reactive force to slow Houston down for tighter controls.
        _reactiveForce = _rb.velocity.normalized * -1 * reactionPower;
        
        //Update the momentum buffer
        momentumBuffer = _rb.velocity * _rb.mass;
        
        //Update Arcade Timers
        jumpBufferTimer += 0.1f;
        //If the BufferTimer exceeds the limit, no longer Buffered.
        if (jumpBufferTimer > jumpBufferLimit)
        {
            jumpBuffered = false;
        }

        coyoteTimer += 0.1f;
    }

    //When landing on the ground
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && other.otherCollider == _foot)
        {
            onGround = true;
            //_rb.velocity = momentumBuffer;
            
            //If we have Buffered our jump when landing, jump
            if (jumpBuffered)
            {
                Jump();
            }
        }
    }

    //When leaving the ground
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && other.otherCollider == _foot)
        {
            onGround = false;
            jumpBuffered = false;
            
            //When we leave the ground, reset the coyote timer
            coyoteTimer = 0.0f;
        }
    }

    public void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, jumpPower);
    }
}
