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
    public static UpgradeHolder Instance;

    [SerializeField] private List<ShopUpgrades> shopObjects;
    
    private UpgradeLevel[] upgradeLevels;
    private List<BoatSkin> boatSkins;
    private BoatSkin activeBoatSkin;
    private CharacterAppearance[] characterAppearances;

    private Player player;
    private PlayerUpgrades playerUpgrades;
    private ShopItemHolder activeBoatItem;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void AddFromSave(SaveData saveData)
    {
        upgradeLevels = saveData.UpgradeLevels;
        boatSkins = saveData.OwnedSkins;
        activeBoatSkin = saveData.EquippedSkin;
        characterAppearances = saveData.CharacterAppearances;
    }
    public void InitializePlayer(Player newPlayer)
    {
        player = newPlayer;
        playerUpgrades = player.GetComponent<PlayerUpgrades>();
        FixUpgrades();
        ApplyCurrentBoatSkin();
        InitializePlayerModel();
    }
    public void InitializePlayerModel()
    {
        player.InitializeModel(characterAppearances);
    }
    public void SetRowerColor(int rowerIndex, RowerColor color)
    {
        characterAppearances[rowerIndex].color = color;
        player.SetRowerColor(rowerIndex, color);
        SaveManager.Instance.SaveToFile();
    }
    public void SetRowerMesh(int rowerIndex, RowerMesh mesh)
    {
        characterAppearances[rowerIndex].mesh = mesh;
        player.SetRowerMesh(rowerIndex, mesh);
        SaveManager.Instance.SaveToFile();
    }
    public void AddUpgrade(UpgradeType upgradeType)
    {
        UpgradeLevel upgradeLevel = upgradeLevels[GetUpgradeIndex(upgradeType)];
        if (Upgrade.MaxLevel <= upgradeLevel.Level)
        {
            return;
        }
        upgradeLevel.IncreaseLevel();
        shopObjects.Find(x => x.UpgradeType == upgradeType).upgradeObject.Upgrade();
        SaveManager.Instance.SaveUpgrades(upgradeLevels);
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
        return (int)type;
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
