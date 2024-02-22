using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct ShopUpgrades
{
    public UpgradeType UpgradeType;
    public Upgrade upgradeObject;

    public ShopUpgrades(UpgradeType UpgradeType, Upgrade upgrade)
    {
        this.UpgradeType = UpgradeType;
        upgradeObject = upgrade;
    }
}

public class UpgradeHolder : MonoBehaviour
{
    [SerializeField] private List<ShopUpgrades> shopObjects;
    [SerializeField] private UpgradeLevel[] upgradeLevels;
    [SerializeField] private List<BoatSkin> boatSkins;
    [SerializeField] private BoatSkin activeBoatSkin;
    public ShopItemHolder activeBoatItem;
    public static UpgradeHolder Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple Upgrade Holders");
            Destroy(this);
        }
    }
    public void AddFromSave(SaveData saveData)
    {
        upgradeLevels = saveData.UpgradeLevels;
        boatSkins = saveData.OwnedSkins;
        var equippedSkin = saveData.EquippedSkin;
        PlayerUpgrades playerUpgrades = FindObjectOfType<PlayerUpgrades>();
        if (equippedSkin != null)
        {
            playerUpgrades.ApplyBoatMaterial(equippedSkin.BoatMaterial);
        }
        else
        {
            playerUpgrades.ApplyBoatMaterial();
        }
        activeBoatSkin = equippedSkin;
        FixUpgrades();
    }
    public void AddUpgrade(UpgradeType upgradeType)
    {
        int index = GetUpgradeIndex(upgradeType);
        if (upgradeLevels[index].Level < Upgrade.MaxLevel)
        {
            upgradeLevels[index].IncreaseLevel();
            shopObjects.Find(x => x.UpgradeType == upgradeType).upgradeObject.Upgrade();
            SaveManager.Instance.SaveUpgrades(upgradeLevels);
        }
    }
    public void ApplyCurrentBoatSkin()
    {
        if (activeBoatSkin != null)
        {
            ApplyBoatSkin(activeBoatSkin);
        }
    }
    public void ApplyBoatShopItem(ShopItemHolder shopItemHolder)
    {
        if (shopItemHolder == null)
        {
            return;
        }
        if (activeBoatItem != null)
        {
            activeBoatItem.CheckMarkSetActivate(false);
        }
        activeBoatItem = shopItemHolder;
        MakeCurrentSkinSelected();
        if (shopItemHolder.Item is BoatSkin boatSkin)
        {
            ApplyBoatSkin(boatSkin);
        }
    }
    private void ApplyBoatSkin(BoatSkin boatSkin)
    {
        if (!boatSkins.Contains(boatSkin))
        {
            boatSkins.Add(boatSkin);
            SaveManager.Instance.SaveSkins(boatSkins);
        }
        FindObjectOfType<PlayerUpgrades>().ApplyBoatMaterial(boatSkin.BoatMaterial);
        SaveManager.Instance.SaveEquippedSkin(boatSkin);
    }
    public void FixUpgrades()
    {
        foreach (UpgradeType type in (UpgradeType[]) Enum.GetValues(typeof(UpgradeType)))
        {
            int index = GetUpgradeIndex(type);
            if (upgradeLevels[index].Level > 0)
            {
                Upgrade upgrade = shopObjects.Find(x => x.UpgradeType == type).upgradeObject;
                upgrade.Upgrade();
            }
        }
    }
    public int GetUpgradeValue(UpgradeType upgradeType)
    {
        Upgrade upgrade = shopObjects.Find(x => x.UpgradeType == upgradeType).upgradeObject;
        return GetUpgradeLevel(upgradeType) * upgrade.valuePerLevel;
    }

    private int GetUpgradeIndex(UpgradeType type)
    {
        for (var index = 0; index < upgradeLevels.Length; index++)
        {
            var upgrade = upgradeLevels[index];
            if (upgrade.UpgradeType == type)
            {
                return index;
            }
        }

        Debug.LogError("Error: Did not find upgrade");
        return -1;
    }
    
    public int GetUpgradeLevel(UpgradeType type)
    {
        int index = GetUpgradeIndex(type);
        return upgradeLevels[index].Level;
    }
    
    public bool HasBoatSkin(BoatSkin skin)
    {
        return boatSkins.Contains(skin);
    }

    public void SelectIfCurrentSkin(ShopItemHolder holder)
    {
        if (holder.Item == activeBoatSkin)
        {
            activeBoatItem = holder;
            MakeCurrentSkinSelected();
        }
        else
        {
            holder.CheckMarkSetActivate(false);
        }
    }
    private void MakeCurrentSkinSelected()
    {
        activeBoatItem.CheckMarkSetActivate(true);
        activeBoatSkin = activeBoatItem.Item as BoatSkin;
    }
}
