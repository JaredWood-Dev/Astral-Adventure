using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Creature : MonoBehaviour
{
    //Creature Class
    //This class contains all the methods and declaration associated with any creature
    //Examples of Creatures: The Player, Enemies, Bosses
    
    [Header("Combat")]
    //The amount of HP a creature has
    public float hitPoints;

    //These reflect a creatures natural resistance or alteration to certain types of damage
    [Header("Damage Adjustments")] 
    //If a creature is resistant, they half the amount they take
    public DamageTypes[] resistances;
    //If a creature is vulnerable, they double the amount they take
    public DamageTypes[] vulnerabilities;
    //If a creature is immune, they take none of the damage
    public DamageTypes[] immunities;
    
    //This game may play with Gravity fields and directions, as such every creature needs to consider the direction of gravity.
    [Header("Gravity")] 
    //This vector stores the current direction of the gravity.
    public Vector2 gravityDirection;

    //Changes the HP of a creature; if the amount reduces the creature to zero, kill the creature
    public void ChangeHitPoints(int amount, DamageTypes type)
    {
        //Check for resistances
        for (int i = 0; i < resistances.Length; i++)
        {
            if (type == resistances[i])
            {
                amount /= 2;
            }
        }
        
        //Check for vulnerabilities
        for (int i = 0; i < vulnerabilities.Length; i++)
        {
            if (type == vulnerabilities[i])
            {
                amount *= 2;
            }
        }
        
        //Check for resistances
        for (int i = 0; i < immunities.Length; i++)
        {
            if (type == immunities[i])
            {
                amount = 0;
            }
        }
        
        if (hitPoints + amount < 1)
        {
                KillCreature();
                return;
        }
        hitPoints += amount;
    }

    //Kills the creature when called
    public void KillCreature()
    {
        //More elegantly later, currently it simply deletes the creature
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        //Ensures the creature is correctly rotated according to the current gravity
        float rot = Mathf.Atan2(gravityDirection.y , gravityDirection.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0 , 0, rot + 90);
        
        //Accelerates the creature according to the current gravity
        gameObject.GetComponent<Rigidbody2D>().AddForce((9.81f * gameObject.GetComponent<Rigidbody2D>().mass) * gravityDirection);
    }
}
