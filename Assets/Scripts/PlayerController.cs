using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] 
    public float movementSpeed;
    public float targetMovementSpeed;
    public Vector2 movementVector;

    [Header("Jumping")] 
    public float jumpPower;
    public bool onGround;
    public float jumpBufferDuration;
    private float _jumpBufferTimer;
    public float coyoteTimeDuration;
    private float _coyoteTimer;
    private float _playerVelocityAgainstGravity; //The player's current velocity projected along the direction opposite to gravity

    [Header("Combat Movement")] 
    public bool isGravSlam;
    public float gravitySlamPower;
    public float fallTime; //How long the player has been performing the gravity slam. Used for damage calculations

    [Header("Combat")] 
    public int attackDamage;
    public float comboTime; //The window of time for a combo to be performed
    private float _comboTimer;
    private int _comboStrikes; //The amount of strikes that have been made in the combo
    public float knockBackPower;

    [Header("Breath Weapon")]
    public float breathPower; //The launch power of Houston's singularity breath
    public float breathDistance;
    public float breathCooldownDuration;
    private float _breathCooldownTimer;
    
    private Rigidbody2D _rb;
    private Creature _c;
    private Animator _animator;
    private Camera _mainCamera;

    private AudioSource _runSound;
    private AudioSource _jumpSound;
    private AudioSource _breathSound;
    private AudioSource _slamSound;
    
    //TODO: IMPLEMENT SINGULARITY BREATH
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _c = GetComponent<Creature>();
        _mainCamera = (Camera)FindObjectOfType(typeof(Camera));
        
        //Assign sound sources
        _runSound = GetComponents<AudioSource>()[3];
        _jumpSound = GetComponents<AudioSource>()[2];
        _breathSound = GetComponents<AudioSource>()[1];
        _slamSound = GetComponents<AudioSource>()[0];
        
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        
        UpdateTimers();

        //Calculates the player's gravity projected along the axis opposite to gravity.
        _playerVelocityAgainstGravity = (_rb.velocity * -_c.gravityDirection).magnitude;

        //If we are in the air and we press the button that is the same direction as gravity, we want to gravity slam
        if ((movementVector == _c.gravityDirection) && !onGround)
        {
            //If we are not already performing a gravity slam
            if (!isGravSlam)
            {
                GravitySlam();
            }
        }
        
        //If we are in the air and performing a gravity slam, and press the button opposite the direction gravity, cancel the gravity slam
        if (movementVector == -_c.gravityDirection && !onGround && isGravSlam)
        {
            EndGravitySlam();
        }

        //If we let the combo time expire, we lose the combo
        if (_comboTimer <= 0)
            _comboStrikes = 0;
        
        //Debug.DrawRay(transform.position, transform.right * (transform.localScale.x * 1000), Color.cyan);
    }

    private void Update()
    {
        //Change the target movement speed based on the input; switches input mode based on which direction gravity is
        //If Houston is upside down, flip the directions of the horizontal movement
        targetMovementSpeed = _c.gravityDirection.y != 0
                ? movementSpeed * Input.GetAxis("Horizontal") * -_c.gravityDirection.y
                : movementSpeed * Input.GetAxis("Vertical") * _c.gravityDirection.x;

            //Update the direction of the player
        //Possibly make this logic better?
        if ((_c.gravityDirection.y != 0 && Input.GetAxis("Horizontal") != 0) || (_c.gravityDirection.y == 0 && Input.GetAxis("Vertical") != 0))
            transform.localScale = new Vector3(Mathf.Sign(targetMovementSpeed), 1, 1);

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
        
        //Update the vector based on your current movement
        movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        //Check for if the player activates Houston's gravity breath and the weapon is not on cooldown
        if (Input.GetButtonDown("Fire3") && _breathCooldownTimer < 0)
            SingularityBreath();
        
        //Handle player input for attacking
        if (Input.GetButtonDown("Fire1"))
            Attack();
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
        if (onGround || (!onGround && Input.GetAxis("Horizontal") != 0 && _c.gravityDirection.y != 0) || (!onGround && Input.GetAxis("Vertical") != 0 && _c.gravityDirection.x != 0))
            _rb.AddForce(f);
        
        //Animate the player
        if (vVector.x != 0)
        {
            //Moving Animation
            _animator.SetBool("directionPressed", true);
            
            //Moving Sound effect
            _runSound.Play();
        }
        else
        {
            //Not Moving Animation
            _animator.SetBool("directionPressed", false);
            
            //Stop the moving sound effect
            _runSound.Stop();
        }
    }

    public void Jump()
    {
        //Play the jump sound effect
        _jumpSound.Play();
        
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
            
            //If we land on the ground while performing a gravity slam
            if (isGravSlam)
            {
                LandGravitySlam();
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
        _breathCooldownTimer -= Time.deltaTime;
        _comboTimer -= Time.deltaTime;
        
        //If a gravity slam is being performed, increment the slam timer
        if (isGravSlam)
            fallTime += Time.fixedDeltaTime;
    }

    //Begins the gravity slam move
    void GravitySlam()
    {
        //Update the condition
        isGravSlam = true;
        
        //Play the animation
        _animator.SetBool("gravSlam", true);
        
        //Delay, then apply the force
        Invoke("ApplySlamForce", 0.2f);
    }

    //Function to allow applying a force after a delay
    void ApplySlamForce()
    {
        _rb.velocity = _c.gravityDirection * gravitySlamPower;
    }

    //Handles stopping a gravity slam. The player will will either air-cancel or follow-through, either will end the Slam.
    void EndGravitySlam()
    {
        //Update the condition
        isGravSlam = false;
        
        //Update the animator
        _animator.SetBool("gravSlam", false);
        
        //Empty the fall timer
        fallTime = 0;
    }

    //Handles the effects of landing while performing a gravity slam
    void LandGravitySlam()
    {
        //Play the sound effect
        _slamSound.Play();
        
        //Use the fallTime variable
        
        //Update necessary changes to stop performing a gravity slam.
        EndGravitySlam();
        
        //Play the particle system
        GetComponents<ParticleSystemController>()[1].StartSystem();
        
        //TODO: IMPLEMENT AOE
    }

    //Houston's breath weapon, a breath that can launch him and launch enemies.
    void SingularityBreath()
    {
        //Play the sound effect
        _breathSound.Play();
        
        //Update the cooldown
        _breathCooldownTimer = breathCooldownDuration;
        
        //If we use the breath., we are most likely not on the ground anymore
        onGround = false;
        
        //Play the particle system
        ParticleSystemController gravityBreathSystem = GetComponents<ParticleSystemController>()[2];
        gravityBreathSystem.StartSystem();
        
        //Next, get the direction of the mouse & Angle the particle system
        Vector3 mousePos = Input.mousePosition;
        Vector2 worldMousePos = _mainCamera.ScreenToWorldPoint(mousePos);
        float diffX = worldMousePos.x - gameObject.transform.position.x;
        float diffY = worldMousePos.y - gameObject.transform.position.y;
        float rot = Mathf.Atan2(diffY,diffX);
        rot = rot * Mathf.Rad2Deg;
        
        //Rotate the particle system accordingly
        gravityBreathSystem.targetSystem.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);

        //Apply AOE damage
        //TODO:IMPLEMENT ATTACK SYSTEM
        //IF CREATURES ARE CLOSE TO HOUSTON, LAUNCH THEM BACK BASED ON DISTANCE
        
        //Raycast Out to mouse
        Vector2 breathDirection = new Vector2(diffX, diffY);
        bool breathRay = Physics2D.Raycast(gameObject.transform.position, breathDirection.normalized, breathDistance, 1 << 7);

        Vector2 breathForce = -breathDirection.normalized * (breathPower * 100);
        //If we hit the ground, launch the player in the opposite direction
        if (breathRay)
            _rb.AddForce(breathForce, ForceMode2D.Impulse);
    }

    //Handles attacking
    //When attacking one click results in a basic strike
    //Three clicks in rapid succession result in a combo that upper cuts the enemy
    void Attack()
    {
        //If we have no combo going, we want to do a basic strike and begin a combo
        if (_comboTimer <= 0)
        {
            ThunderGauntletStrike();
            _comboTimer = comboTime;
            _comboStrikes++;
        }
        //If we have a combo going but not enough strikes for a uppercut also just do a thunder gauntlet strike
        else if (_comboTimer > 0 && _comboStrikes < 2)
        {
            ThunderGauntletStrike();
            _comboStrikes++;
        }
        //If we have a combo going and enough strikes, we upper cut then end the combo
        else if (_comboTimer > 0 && _comboStrikes >= 2)
        {
            UpperCut();
            _comboStrikes = 0;
            _comboTimer = 0;
        }
    }

    void ThunderGauntletStrike()
    {
        RaycastHit2D target = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, 5, 1 << 8);
        //print(transform.right * transform.localScale.x);
        if (target)
        {
            target.collider.gameObject.GetComponent<Creature>().HitCreature(attackDamage, DamageTypes.Thunder, transform.right * (transform.localScale.x * knockBackPower));
        }
    }

    void UpperCut()
    {
        RaycastHit2D target = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, 5, 1 << 8);
        //print(transform.right * transform.localScale.x);
        if (target)
        {
            target.collider.gameObject.GetComponent<Creature>().HitCreature(attackDamage, DamageTypes.Thunder, transform.up * (500 * knockBackPower));
        }
    }
}
