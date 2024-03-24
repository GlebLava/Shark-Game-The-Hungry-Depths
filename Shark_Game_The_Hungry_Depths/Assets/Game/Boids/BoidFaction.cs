using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BoidFaction")]
public class BoidFaction : ScriptableObject
{
    public GameObject boidPrefab;

    [Header("Speed and Steering")]
    public float maxSpeed = 7f;
    public float maxSteerForce = 1f;

    [Header("Radii (ALL SQUARED)")]
    public float radiusPlayerDetection = 1.5f;
    
    
    public float radiusBoidsAvoidance = 2f;
    public float radiusVelocityMatching = 3f;
    public float radiusCentring = 4.5f;

    [Header("Weights")]
    public float collidersAvoidanceWeight = 7f;
    public float playerAvoidanceWeight = 10f;


    public float boidsAvoidanceWeight = 1.2f;
    public float velocityMatchingWeight = 1.6f;
    public float flockCenteringWeight = 1f;
}


