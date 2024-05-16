using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour
{
    //This script causes any gravity object that enters it to change the direction of gravity
    public Vector2 fieldDirection = new(0,-1);

    private void OnTriggerEnter2D(Collider2D other)
    {
        //UpdateGravity(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        UpdateGravity(other);
    }

    void UpdateGravity(Collider2D other)
    {
        if (other == null) return;
                //If the other object is effected by gravity
                if (other.gameObject.GetComponent<GravityObject>() != null)
                {
                    //Set the direction of the gravity to that object
                    other.gameObject.GetComponent<GravityObject>().gravityDirection = fieldDirection;
                }
    }
}
