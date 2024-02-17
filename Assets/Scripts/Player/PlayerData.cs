using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "ScriptableObjects/Player/Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    public List<Material> maleColors = new();
    public List<Material> femaleColors = new();
    public List<Material> androgynousColors = new();

    public List<Material> GetChosenHairStyleMaterials(int hairStyle)
    {
        switch (hairStyle)
        {
            case 0:
                return maleColors;
            case 1:
                return femaleColors;
            case 2:
                return androgynousColors;
            default:
                Debug.LogError("DOES NOT EXIST!!!");
                break;
        }

        return null;
    }
}
