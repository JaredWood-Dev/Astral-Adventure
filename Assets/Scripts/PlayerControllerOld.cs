using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerOld : MonoBehaviour
{
    //This script handles the player movement and other controls
    public Camera mainCamera;

    [Header("Movement")]
    //The maximum speed of Houston
    public float maxSpeed;
    //The jumping strength of Houston
    public float jumpPower;
    public float reactionPower;
    public bool onGround;
    private bool _horizontal = true;

    [Header("Arcade Gameplay")] 
    public Vector2 momentumBuffer;
    public float jumpBufferTimer = 0.0f;
    public float jumpBufferLimit = 0.1f;
    public bool jumpBuffered;
    public float coyoteTimer = 0.0f;
    public float coyoteLimit = 0.1f;

    [Header("Combat")] 
    public bool isPounding = false;
    public float poundPower = 10;

    private float _movementForce;
    private Vector2 _reactiveForce;
    private Rigidbody2D _rb;
    private Animator _an;
    private Collider2D _foot;
    private Creature _c;

    [Header("Gravity Breath")] 
    public float launchPower;
    public float breathDistance = 5;
    public float breathCoolDown = 10f;
    public float breathTimer;
    
    //Particle Systems
    private ParticleSystemController _runSystem;
    private ParticleSystemController _gravPoundSystem;
    private ParticleSystemController _gravityBreathSystem;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _an = gameObject.GetComponent<Animator>();
        _foot = gameObject.GetComponents<Collider2D>()[1];
        _c = gameObject.GetComponent<Creature>();
        _runSystem = gameObject.GetComponents<ParticleSystemController>()[0]; //First Particle System is the run
        _gravPoundSystem = gameObject.GetComponents<ParticleSystemController>()[1]; //Second Particle System is the shockwave
        _gravityBreathSystem = gameObject.GetComponents<ParticleSystemController>()[2]; //Third Particle System is Houston's Gravity breath

    }

    void Update()
    {
        //Houston's Breath Weapon
        if (Input.GetKeyDown("left shift"))
        {
            if (breathTimer > breathCoolDown)
            {
                BreathWeapon();
                breathTimer = 0.0f;
            }
        }
        
        
        //When pressing a directional key, add the force, scale the appreciate direction, and update the animations
        //If the direction gravity is "vertical enough" use horizontal movement
        if (_horizontal)
        {
            //X Axis Movement 
            if (Input.GetKey("d"))
            {
                _rb.AddForce(_movementForce * RotateVector2(Vector2.up, -Mathf.PI/2));
                gameObject.transform.localScale = new Vector3(-_c.gravityDirection.y, 1, 1);
                _an.SetBool("directionPressed", true);
            }
            else if (Input.GetKey("a"))
            {
                _rb.AddForce(_movementForce * RotateVector2(Vector2.up, Mathf.PI/2));
                gameObject.transform.localScale = new Vector3(_c.gravityDirection.y, 1, 1);
                _an.SetBool("directionPressed", true);
                
            }
            else
            {
                _an.SetBool("directionPressed", false);

            }
            
            //Check for Thunder-Pound
            if (Input.GetKeyDown("w") && _c.gravityDirection.y > 0 && !onGround)
            {
                isPounding = true;
                ThunderGauntletPound();
            }
            if (Input.GetKey("s") && isPounding && _c.gravityDirection.y > 0)
            {
                isPounding = false;
                _an.SetBool("gravSlam", false);
            }

            if (Input.GetKeyDown("s") && _c.gravityDirection.y < 0 && !onGround)
            {
                isPounding = true;
                ThunderGauntletPound();
            }
            if (Input.GetKey("w") && isPounding && _c.gravityDirection.y < 0)
            {
                isPounding = false;
                _an.SetBool("gravSlam", false);
            }

            if (Mathf.Abs(_rb.velocity.x) > 0)
            {
                _runSystem.StartSystem();
            }
            else
            {
                _runSystem.StopSystem();
            }
        }
        else
        {
            //Y Axis Movement
            if (Input.GetKey("s"))
            {
                _rb.AddForce(_movementForce * Vector2.down);
                gameObject.transform.localScale = new Vector3( -_c.gravityDirection.x, 1, 1);
                _an.SetBool("directionPressed", true);
            }
            else if (Input.GetKey("w"))
            {
                _rb.AddForce(_movementForce * Vector2.up);
                gameObject.transform.localScale = new Vector3(_c.gravityDirection.x, 1, 1);
                _an.SetBool("directionPressed", true);
            }
            else
            {
                _an.SetBool("directionPressed", false);
            }
            
            //Check for Thunder-Pound
            if (Input.GetKeyDown("d") && _c.gravityDirection.x > 0 && !onGround)
            {
                isPounding = true;
                ThunderGauntletPound();
            }
            if (Input.GetKey("a") && isPounding && _c.gravityDirection.x > 0)
            {
                isPounding = false;
                _an.SetBool("gravSlam", false);
            }

            if (Input.GetKeyDown("a") && _c.gravityDirection.x < 0 && !onGround)
            {
                isPounding = true;
                ThunderGauntletPound();
            }
            if (Input.GetKey("d") && isPounding && _c.gravityDirection.x < 0)
            {
                isPounding = false;
                _an.SetBool("gravSlam", false);
            }
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
        
        //Variable Jump Hight: Resetting back to normal gravity
        if (Input.GetKeyUp("space") || (_rb.velocity * -_c.gravityDirection).magnitude < 0.1)
        {
            _c.currentGravityAcceleration = _c.defaultGravityAcceleration;
        }

        //Reactive force, for tighter gameplay
        if (onGround)
        {
            _rb.AddForce(_reactiveForce);
        }
    }

    private void FixedUpdate()
    {
        //We need to calculate the amount of force needed to accelerate Houston to the desired maximum speed
        //F = m * a, a = v0 - vf / t
        if (_horizontal)
        {
            if (onGround)
            {
                _movementForce = _rb.mass * (Mathf.Abs(maxSpeed - Mathf.Abs(_rb.velocity.x) / (Time.fixedDeltaTime * 100)));
            }
            else
            {
                _movementForce = 0.25f * _rb.mass * (Mathf.Abs(maxSpeed - Mathf.Abs(_rb.velocity.x) / (Time.fixedDeltaTime * 100)));
            }
        }
        else
        {
            if (onGround)
            {
                _movementForce = _rb.mass * (Mathf.Abs(maxSpeed - Mathf.Abs(_rb.velocity.y) / (Time.fixedDeltaTime * 100)));
            }
            else
            {
                _movementForce = 0.25f * _rb.mass * (Mathf.Abs(maxSpeed - Mathf.Abs(_rb.velocity.y) / (Time.fixedDeltaTime * 100)));
            }
        }
        
        //Clamp just in case, no absurd forces silly physics
        _movementForce = Mathf.Clamp(_movementForce, -1500, 1500);

        //This calculates the reactive force to slow Houston down for tighter controls.
        _reactiveForce = _rb.velocity.normalized * -1 * reactionPower;
        
        //Update the momentum buffer
        momentumBuffer = _rb.velocity;
        
        //Update Arcade Timers
        jumpBufferTimer += 0.1f;
        //If the BufferTimer exceeds the limit, no longer Buffered.
        if (jumpBufferTimer > jumpBufferLimit)
        {
            jumpBuffered = false;
        }

        coyoteTimer += 0.1f;


        if (Mathf.Abs(_c.gravityDirection.y) >= Mathf.Sin(Mathf.PI/4))
        {
            _horizontal = true;
        }
        else
        {
            _horizontal = false;
        }
        
        //Update Cooldown
        breathTimer += 0.1f;
    }

    //When landing on the ground
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && other.otherCollider == _foot)
        {
            onGround = true;
            _rb.velocity += momentumBuffer;

            //If we have Buffered our jump when landing, jump
            if (jumpBuffered)
            {
                Jump();
            }
            
            //TODO IMPLEMENT GAUNTLET SHOCKWAVE DAMAGE
            if (isPounding)
            {
                isPounding = false;
                _gravPoundSystem.StartSystem();
            }
            
            _an.SetBool("inAir", false);
            _an.SetBool("gravSlam", false);
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
        if (_horizontal)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpPower * -_c.gravityDirection.y);
        }
        else
        {
            _rb.velocity = new Vector2(jumpPower * -_c.gravityDirection.x , _rb.velocity.y);
        }

        _c.currentGravityAcceleration = _c.defaultGravityAcceleration / 2;
        
        _an.SetBool("inAir", true);
    }

    public Vector2 RotateVector2(Vector2 n, float angle)
    {
        float newX = n.x * Mathf.Cos(angle) - n.y * Mathf.Sin(angle);
        float newY = n.x * Mathf.Sin(angle) - n.y * Mathf.Cos(angle);
        return new Vector2(newX, newY);
    }

    public void ThunderGauntletPound()
    {
        _rb.AddForce(jumpPower * -_c.gravityDirection, ForceMode2D.Impulse);
        _an.SetBool("gravSlam", true);
        Invoke("FinishPound", 0.2f);
    }

    private void FinishPound()
    {
        _rb.velocity = _c.gravityDirection * poundPower;
    }

    void BreathWeapon()
    {
        //Play the particle system
        _gravityBreathSystem.StartSystem();
        
        //Next, get the direction of the mouse & Angle the particle system
        Vector3 mousePos = Input.mousePosition;
        Vector2 worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);
        float diffX = worldMousePos.x - gameObject.transform.position.x;
        float diffY = worldMousePos.y - gameObject.transform.position.y;
        float rot = Mathf.Atan2(diffY,diffX);
        rot = rot * Mathf.Rad2Deg;
        
        _gravityBreathSystem.targetSystem.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);

        //Apply AOE damage
        //TODO:IMPLEMENT ATTACK SYSTEM
        
        //Raycast Out to mouse
        Vector2 breathDirection = new Vector2(diffX, diffY);
        bool breathRay = Physics2D.Raycast(gameObject.transform.position, breathDirection.normalized, breathDistance, 1 << 7);

        //If we hit the ground, launch the player in the opposite direction
        if (breathRay)
        {
            _rb.AddForce(-breathDirection.normalized * launchPower * 100, ForceMode2D.Impulse);
        }
    }
}
