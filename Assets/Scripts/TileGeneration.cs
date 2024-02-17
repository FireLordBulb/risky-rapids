using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileGeneration : MonoBehaviour
{

    
    
    [SerializeField] private Transform floorParent;
    [SerializeField] private List<GameObject> tiles;

    [Space] [SerializeField] private float timeBetweenTileSpawn;
    private float currentTime;
    
}
