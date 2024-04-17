using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] 
    public float movementSpeed;
    public float targetMovementSpeed;
    [Range(1,2)]
    public float reactionPower;

    private Rigidbody2D _rb;
    private Creature _c;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _c = GetComponent<Creature>();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
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
    }

    public void MoveCharacter()
    {
        //Get the velocity relative to the player
        Vector2 vVector = transform.InverseTransformDirection(_rb.velocity);

        //Calculate the force necessary to move the character
        float a = (targetMovementSpeed - vVector.x) / Time.deltaTime;

        //Calculate the force vector
        Vector2 f = a * _rb.mass * transform.right * reactionPower;
        
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
}
