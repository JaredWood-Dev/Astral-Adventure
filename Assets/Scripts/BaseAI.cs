using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        _possibleTargets = new List<GameObject>();
    }


    /*
     * When called, it attempts to locate a target creature.
     * It gets all creatures, narrows it by distance and tag,
     * then chooses the closest one.
     */
    void TargetCreature()
    {
        _possibleTargets.Clear();
        //print("Attempting to find a Target");
        foreach (var _tag in targetableTags)
        {
            foreach (var creature in GameObject.FindGameObjectsWithTag(_tag))
            {
                if (Vector2.Distance(gameObject.transform.position, creature.transform.position) < aggressionDistance)
                {
                    _possibleTargets.Add(creature.gameObject);
                }
            }
        }
        
        if (_possibleTargets.Count < 1)
        {
            target = null;
            return;
        }

        GameObject closestCreature = null;
        float min = Single.PositiveInfinity;
        foreach (var creature in _possibleTargets)
        {
            float dist = Vector2.Distance(gameObject.transform.position, creature.transform.position);
            if (dist < min)
            {
                min = dist;
                closestCreature = creature;
            }
        }

        target = closestCreature;
    }
}
