using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaniteChaseAI : BaseAI
{
    /*
     * This script is for the basic nanite chaser enemy.
     * This enemy, chases the player around the map, it is capable of climbing, but not jumping.
     */

    public float movementSpeed;
    public int attackDamage;

    private Creature _c;
    private Rigidbody2D _rb;
    private Animator _a;

    void Awake()
    {
        _c = GetComponent<Creature>();
        _rb = GetComponent<Rigidbody2D>();
        _a = GetComponent<Animator>();
    }

    private void Update()
    {
        //If a creature is being targeted
        if (target)
        {
            //If horizontal
            if (_c.gravityDirection.y != 0)
            {
                if (target.gameObject.transform.position.x > gameObject.transform.position.x)
                {
                    Move(transform.right);
                }
                else
                {
                    Move(-transform.right);
                }
            }

            //If vertical
            if (_c.gravityDirection.x != 0)
            {
                if (target.gameObject.transform.position.y > gameObject.transform.position.y)
                {
                    Move(transform.up);
                }
                else
                {
                    Move(-transform.up);
                }
            }
        }
    }

    private void Move(Vector2 direction)
    {
        //Animate
        _a.SetBool("isMoving", true);
        
        print("Moving!" + direction);
        
        //Move the nanites
        _rb.velocity = direction * movementSpeed;

        //Stop movement aniamtion
        //_a.SetBool("isMoving", false);
    }
}
