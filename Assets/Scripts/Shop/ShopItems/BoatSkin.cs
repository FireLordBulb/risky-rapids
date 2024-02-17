using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/Shop/ShopItems/BoatSkin", order = 1)]
public class BoatSkin : ShopItem
{
    public Material BoatMaterial;
    
    public override void Upgrade()
    {
        Debug.Log("I bought skin");
    }
}
