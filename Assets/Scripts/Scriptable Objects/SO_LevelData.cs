using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "ScriptableObjects/Level Data")]
public class SO_LevelData : ScriptableObject
{
    public new string name;
    public float baseTime;
    public int highScore;
}
