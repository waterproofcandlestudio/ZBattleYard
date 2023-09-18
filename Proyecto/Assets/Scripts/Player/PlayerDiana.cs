using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// To let player be followed by enemies
/// 
/// </summary>
public class PlayerDiana : MonoBehaviour
{
    public static Transform instance; // Instance of player so enemy targets hims when they spawnin in the map

    void Awake() => instance = this.transform;
}
