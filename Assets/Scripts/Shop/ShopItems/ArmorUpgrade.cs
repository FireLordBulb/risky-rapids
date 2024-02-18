using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/Shop/ShopItems/Upgrades/Armor", order = 1)]
public class ArmorUpgrade : Upgrade
{
    public PlayerHealth health;
    public override void Upgrade()
    {
        Debug.Log("Armor buy");
        PlayerUpgrades player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerUpgrades>();
        player.ActivateArmor();
        FindObjectOfType<Player>().UpgradePlayerArmor();
    }
}
