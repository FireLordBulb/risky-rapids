using System.Collections.Generic;
using UnityEngine;

public class UpgradeHolder : MonoBehaviour
{
    public static UpgradeHolder Instance;

    [SerializeField] private List<Upgrade> upgrades;
    
    private UpgradeLevel[] upgradeLevels;
    private List<BoatSkin> boatSkins;
    private BoatSkin activeBoatSkin;
    private CharacterAppearance[] characterAppearances;

    private Player player;
    private PlayerUpgrades playerUpgrades;
    private ShopItemHolder activeBoatItem;
    
    public BoatSkin ActiveBoatSkin => activeBoatSkin;
    public PlayerUpgrades PlayerUpgrades => playerUpgrades;

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
        if (player != null)
        {
            ApplySaveData();
        }
    }
    public void InitializePlayer(Player newPlayer)
    {
        player = newPlayer;
        playerUpgrades = player.GetComponent<PlayerUpgrades>();
        if (upgradeLevels != null)
        {
            ApplySaveData();
        }
    }

    private void ApplySaveData()
    {
        FixUpgrades();
        if (activeBoatSkin != null)
        {
            ApplyBoatSkin(activeBoatSkin);
        }
        player.InitializeModel(characterAppearances);
    }
    private void FixUpgrades()
    {
        foreach (UpgradeLevel upgradeLevel in upgradeLevels)
        {
            if (upgradeLevel.level > 0)
            {
                Upgrade upgrade = upgrades[(int)upgradeLevel.upgradeType];
                upgrade.Upgrade();
            }
        }
    }
    public void SetRowerColor(int rowerIndex, RowerColor color)
    {
        characterAppearances[rowerIndex].color = color;
        player.SetRowerColor(rowerIndex, color);
        SaveManager.Instance.Save();
    }
    public void SetRowerMesh(int rowerIndex, RowerMesh mesh)
    {
        characterAppearances[rowerIndex].mesh = mesh;
        player.SetRowerMesh(rowerIndex, mesh);
        SaveManager.Instance.Save();
    }
    public void AddUpgrade(UpgradeType upgradeType)
    {
        UpgradeLevel upgradeLevel = upgradeLevels[(int)upgradeType];
        if (Upgrade.MaxLevel <= upgradeLevel.level)
        {
            return;
        }
        upgradeLevel.IncreaseLevel();
        upgrades[(int)upgradeType].Upgrade();
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
        }
        playerUpgrades.ApplyBoatMaterial(boatSkin.BoatMaterial);
    }
    public int GetUpgradeValue(UpgradeType upgradeType)
    {
        Upgrade upgrade = upgrades[(int)upgradeType];
        return GetUpgradeLevel(upgradeType) * upgrade.valuePerLevel;
    }
    public int GetUpgradeLevel(UpgradeType type)
    {
        return upgradeLevels[(int)type].level;
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
