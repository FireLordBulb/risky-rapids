using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ShopItem : ScriptableObject
{
    public string Title;
    public string Description;
    public int Cost;
    public SO_GameData GameData;
    public TMP_Sprite ItemPicture;
    public TMP_Sprite ItemTemplate;

    public abstract void Upgrade();

}
