using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SharkSO")]
public class SharkSO : ScriptableObject
{
    public GameObject sharkModelPrefab;
    public int cost;
    public float moveSpeed;
    public float camDistance;
    public float camYOffset;
}
