using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] 
    public float movementSpeed;
    public float targetMovementSpeed;

    [Header("Jumping")] 
    public float jumpPower;
    public bool onGround;
    public float jumpBufferDuration;
    private float _jumpBufferTimer;
    public float coyoteTimeDuration;
    private float _coyoteTimer;
    private float _playerVelocityAgainstGravity; //The player's current velocity projected along the direction opposite to gravity

    private Rigidbody2D _rb;
    private Creature _c;
    private Animator _animator;

    //TODO: IMPLEMENT VARIABLE JUMP HEIGHT
    //TODO: IMPLEMENT GRAVITY SLAM
    //TODO: IMPLEMENT SINGULARITY BREATH
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _c = GetComponent<Creature>();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        
        UpdateTimers();

        //Calculates the player's gravity projected along the axis opposite to gravity.
        _playerVelocityAgainstGravity = (_rb.velocity * -_c.gravityDirection).magnitude;
    }

    private void Update()
    {
        //Change the target movement speed based on the input
        targetMovementSpeed = movementSpeed * Input.GetAxis("Horizontal");
        
        //Update the direction of the player
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(targetMovementSpeed), 1, 1);
        }
        
        //If the key is pressed to jump
        if (Input.GetButtonDown("Jump"))
        {
            //Make sure we are on the ground before jumping
            if (onGround || Physics2D.OverlapBox(transform.position, new Vector2(0.45f,1), transform.position.z, 1<<7))
            {
                //Then jump
                Jump();
            }
            //If we are not on the ground, but have recently left the ground, jump anyway (Coyote Time)
            else if(_coyoteTimer > 0)
            {
                Jump();
            }
            //If we are are not on the ground, buffer the jump
            else
            {
                _jumpBufferTimer = jumpBufferDuration;
            }
        }
        
        //Variable Jump Height: If we release the space bar, or reach the apex, return to normal gravity.
        if (Input.GetButtonUp("Jump") || (_playerVelocityAgainstGravity < 1 && !onGround))
        {
            _c.currentGravityAcceleration = _c.defaultGravityAcceleration;
        }
    }

    public void MoveCharacter()
    {
        //Get the velocity relative to the player
        Vector2 vVector = transform.InverseTransformDirection(_rb.velocity);

        //Calculate the force necessary to move the character
        float a = (targetMovementSpeed - vVector.x) / Time.deltaTime;

        //Calculate the force vector
        Vector2 f = a * _rb.mass * transform.right;
        
        //Adjust the power based on if we are in the air
        if (!onGround)
            f *= 0.25f;
        
            //Apply the force to the character
        _rb.AddForce(f);
        
        //Animate the player
        if (vVector.x != 0)
        {
            //Moving Animation
            _animator.SetBool("directionPressed", true);
        }
        else
        {
            //Not Moving Animation
            _animator.SetBool("directionPressed", false);
        }
    }

    public void Jump()
    {
        //When we jump, we want to apply an impulse in the opposite direction of gravity
        _rb.AddForce(jumpPower * transform.up, ForceMode2D.Impulse);
        
        //Variable Jump Height: Adjust the gravity when we jump
        //While the space bar is pressed, half the downward acceleration of gravity
        _c.currentGravityAcceleration = _c.defaultGravityAcceleration / 2;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //If we land on the ground and the collider that hit the ground is the foot collider
        if (other.gameObject.CompareTag("Ground") && other.otherCollider == gameObject.GetComponents<BoxCollider2D>()[1])
        {
            onGround = true;
            _animator.SetBool("inAir", false);

            //If the jump is buffered, we want to follow through
            if (_jumpBufferTimer > 0)
            {
                Jump();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        //If we are leaving ground, we are no longer onGround
        if (other.gameObject.CompareTag("Ground") && other.otherCollider == gameObject.GetComponents<BoxCollider2D>()[1])
        {
            onGround = false;
            _animator.SetBool("inAir", true);
            
            //If we leave the ground we also want to start our coyote timer
            _coyoteTimer = coyoteTimeDuration;
        }
    }
    
    //This function handles updating important timers
    void UpdateTimers()
    {
        _jumpBufferTimer -= Time.fixedDeltaTime;
        _coyoteTimer -= Time.fixedDeltaTime;
    }
}
