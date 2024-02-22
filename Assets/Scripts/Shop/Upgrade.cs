using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Upgrade : ShopItem
{
    public static int MaxLevel = 3;
    public UpgradeType UpgradeType;
    public int valuePerLevel;
}
