using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Creature : GravityObject
{
    //Creature Class
    //This class contains all the methods and declaration associated with any creature
    //Examples of Creatures: The Player, Enemies, Bosses
    
    [Header("Combat")]
    //The amount of HP a creature has
    public float hitPoints;
    [Range(-1,1)]
    //How well the creature resists knockback. When struck the creature will multiply the distance by one minus this number
    public float knockBackResistance;

    //These reflect a creatures natural resistance or alteration to certain types of damage
    [Header("Damage Adjustments")] 
    //If a creature is resistant, they half the amount they take
    public DamageTypes[] resistances;
    //If a creature is vulnerable, they double the amount they take
    public DamageTypes[] vulnerabilities;
    //If a creature is immune, they take none of the damage
    public DamageTypes[] immunities;
    

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
        //More elegantly later, currently it simply deletes the creature747
        Destroy(gameObject);
    }
    
    //Damages the creature by the specified amount and adds knockback.
    public void HitCreature(int damageAmount, DamageTypes type, Vector2 knockBack)
    {
        //Adjusts the hit-points
        ChangeHitPoints(damageAmount, type);
        
        //Apply the knock-back
        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().AddForce(knockBack * (1 - knockBackResistance));
    }
    
}
