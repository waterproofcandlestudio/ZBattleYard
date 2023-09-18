using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuy : MonoBehaviour
{
    [SerializeField] new string name = "Wall";
    [SerializeField] int price;

    [Header("Spawners to activate after buying wall!")]
    [SerializeField] Transform[] spawners;

    [Header("Walls connected to this one that must be opened at the same time")]
    [SerializeField] Transform[] connectedWalls;

    public string GetWallName() => name;
    public int GetWallPrice() => price;
    void ActivateSpawners()
    {
        foreach (Transform spawn in spawners)
        {
            if (spawn == null)
                return;

            spawn.gameObject.SetActive(true);
        }
    }
    void OpenConnectedWalls()
    {
        foreach (Transform wall in connectedWalls)
        {
            if (wall == null)
                return;

            wall.gameObject.SetActive(false);        
        }
    }
    void OnDisable()
    {
        ActivateSpawners();
        OpenConnectedWalls();
    }
}
