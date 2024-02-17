using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct ShopUpgrades
{
    public UpgradeType UpgradeType;
    public Upgrades UpgradeObject;

    public ShopUpgrades(UpgradeType UpgradeType, Upgrades upgrades)
    {
        this.UpgradeType = UpgradeType;
        UpgradeObject = upgrades;
    }
}

public class UpgradeHolder : MonoBehaviour
{
    [SerializeField] private List<ShopUpgrades> shopObjects;
    [SerializeField] private List<UpgradeLevel> upgradeLevels = new();
    [SerializeField] private List<BoatSkin> boatSkins = new();
    [SerializeField] private BoatSkin activeBoatSkin;
    public ShopItemHolder ActiveBoatItem;
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
    public void AddFromSave(List<UpgradeLevel> levels, List<BoatSkin> savedBoatSkins, BoatSkin equippedSkin)
    {
        upgradeLevels.AddRange(levels);
        boatSkins.AddRange(savedBoatSkins);
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
        if (upgradeLevels[index].Level < upgradeLevels[index].MaxLevel)
        {
            upgradeLevels[index].IncreaseLevel();
            shopObjects.Find(x => x.UpgradeType == upgradeType).UpgradeObject.Upgrade();
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
        if (ActiveBoatItem != null)
        {
            ActiveBoatItem.DisableCheckMark();
        }
        ActiveBoatItem = shopItemHolder;
        MakeCurrentSkinSelected();
        if (shopItemHolder.GetItem() is BoatSkin boatSkin)
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
                Upgrades upgrade = shopObjects.Find(x => x.UpgradeType == type).UpgradeObject;
                upgrade.Upgrade();
            }
        }
    }
    public int GetUpgradeValue(UpgradeType upgradeType)
    {
        Upgrades upgrade = shopObjects.Find(x => x.UpgradeType == upgradeType).UpgradeObject;
        return GetUpgradeLevel(upgradeType) * upgrade.valuePerLevel;
    }

    public int GetUpgradeIndex(UpgradeType type)
    {
        for (var index = 0; index < upgradeLevels.Count; index++)
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
        if (holder.GetItem() == activeBoatSkin)
        {
            ActiveBoatItem = holder;
            MakeCurrentSkinSelected();
        }
        else
        {
            holder.DisableCheckMark();
        }
    }
    private void MakeCurrentSkinSelected()
    {
        ActiveBoatItem.ActivateCheckMark();
        activeBoatSkin = ActiveBoatItem.GetItem() as BoatSkin;
    }
}
