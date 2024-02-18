using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/Shop/ShopItems/Upgrades/Magnet", order = 1)]
public class MagnetUpgrade : Upgrade
{
    public override void Upgrade()
    {
        Debug.Log("Magnet Buy");
        PlayerUpgrades player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerUpgrades>();
        player.ActivateMagnet();
    }
}
