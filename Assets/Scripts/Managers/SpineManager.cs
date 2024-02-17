using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Unity.VisualScripting;
using UnityEngine;

public class SplineManager : MonoBehaviour
{
    public static SplineManager Instance;
    [Header("Place the splines IN ORDER")]
    [SerializeField] private SplineComputer[] splines;
    public Vector3[] Positions {get; private set;}
    public Vector3[] Directions {get; private set;}
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        } 
        Instance = this; 
        List<Vector3> positionList = new();
        foreach (SplineComputer spline in splines)
        {
            positionList.AddRange(spline.GetPoints().Select(point => point.position));
        }
        Positions = positionList.ToArray();
        Directions = new Vector3[Positions.Length];
        for (int i = 0; i < Directions.Length-1; i++){
            Directions[i] = (Positions[i+1]-Positions[i]).normalized;
        }
        // The last point has no direction since there is no next point to point towards.
        if (0 < Directions.Length) Directions[^1] = Vector3.zero;
    }
}
