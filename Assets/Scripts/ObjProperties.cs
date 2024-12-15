using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ObjProperties : MonoBehaviour
{
    [Header("Properties that affect collisions")]
    [Tooltip("Sends the player back to their previous position upon overlap")]
    public bool wall;
    [Tooltip("Kills the player or decrements their health")]
    public bool hazard;
    [Tooltip("Causes bullets to go in the opposite direction")]
    public bool ricochet;
    [Tooltip("Is destructable by bullets")]
    public bool destructable;
    [Tooltip("Is destructable by player")]
    public bool fragile;
    [Tooltip("Reflects any bullets")]
    public bool reflective;
}
