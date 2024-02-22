using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/Shop/ShopItems/Upgrades/Row", order = 1)]
public class SpeedPaddleUpgrade : Upgrade
{
    public override void Upgrade()
    {
        PlayerUpgrades.UpdatePaddleMaterial();
        PlayerUpgrades.GetComponent<BoatPhysics>().AddSpeedPaddle();
    }
}