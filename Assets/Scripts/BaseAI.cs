using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseAI : MonoBehaviour
{
    /*
     * Base AI is intended to be the superclass to all of the AI scripts.
     * It will have very little code, and provide a framework for other AIs to inherit.
     * However, this will not be an interface since some code implementation will be done.
     */

    //The distance at which the creature will target onto another creature
    public float aggressionDistance;
    //The current target of the creature
    public GameObject target = null;
    //A list of the tags that the creature can target. For enemies this will usually only contain "player"
    public List<string> targetableTags;
    //A list of all the targets
    private List<GameObject> _possibleTargets;

    private void Start()
    {
        InvokeRepeating("TargetCreature", 0.5f, 0.5f);
    }


    /*
     * When called, it attempts to locate a target creature.
     * It gets all creatures, narrows it by distance and tag,
     * then chooses the closest one.
     */
    void TargetCreature()
    {
        print("Attempting to find a Target");
        print(GameObject.FindGameObjectsWithTag("Player").Length);
        foreach (var itag in targetableTags)
        {
            if(GameObject.FindGameObjectsWithTag(itag).Length > 0)
                _possibleTargets.AddRange(GameObject.FindGameObjectsWithTag(itag));
        }
        
        float targetDistance;
        float smallestDistance = 0;
        GameObject closestTarget = null;
        //Choose the closest target, if it is within range.
        foreach (var creature in _possibleTargets)
        {
            targetDistance = Vector2.Distance(gameObject.transform.position, creature.transform.position);
            if (targetDistance > smallestDistance)
            {
                smallestDistance = targetDistance;
                closestTarget = creature;
            }
        }

        if (smallestDistance > aggressionDistance)
        {
            target = null;
        }
        else
        {
            target = closestTarget;
        }
    }
}
