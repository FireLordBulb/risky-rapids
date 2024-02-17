using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/Shop/ShopItems/Upgrades/Row", order = 1)]
public class SpeedPaddleUpgrade : Upgrades
{
    
    public override void Upgrade()
    {
        Debug.Log("Row Buy");
        PlayerUpgrades player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerUpgrades>();
        player.ActivateRow();
        FindObjectOfType<BoatPhysics>().AddSpeedPaddle(valuePerLevel * UpgradeHolder.Instance.GetUpgradeLevel(UpgradeType.Paddle));
    }
}