using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Obstacle Data", menuName = "ScriptableObjects/Obstacle Data", order = 1)]
public class ObstacleData : ScriptableObject
{
    public float minimumDamage;
    public float maximumDamage;
    [Space]
    public float knockback;
    public float minSpeedScalar;

}
