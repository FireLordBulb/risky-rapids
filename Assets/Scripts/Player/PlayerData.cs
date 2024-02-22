using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "ScriptableObjects/Player/Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private MaterialArray[] materialArrays;

    public Material GetMaterial(CharacterMesh mesh, CharacterColor color)
    {
        return materialArrays[(int)mesh].materials[(int)color];
    }
}
