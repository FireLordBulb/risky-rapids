using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Data", menuName = "ScriptableObjects/Game Data", order = 1)]
public class SO_GameData : ScriptableObject
{
    public float GameLength;
    public float MaxHealth;
    public float Armor;
}
