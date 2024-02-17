using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavedStats
{
    public int Coins;
    
    // Upgrades and skins
    public List<UpgradeLevel> UpgradeLevels;
    public List<BoatSkin> SkinsUnlocked;
    public BoatSkin EquippedBoat;
}
[Serializable]
public class UpgradeLevel
{
    public UpgradeType UpgradeType;
    public int Level;
    public int MaxLevel = 3; // Should not be changed, readonly does not work

    public UpgradeLevel(UpgradeType upgradeType)
    {
        UpgradeType = upgradeType;
        Level = 0;
    }

    public void IncreaseLevel()
    {
        if (Level < MaxLevel)
        {
            Debug.Log("Level up");
            Level++;
        }
    }
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    [SerializeField] private string fileName = "Save";
    
    private UpgradeHolder upgradeHolder;
    private SavedStats savedStats;
    private string saveDirectoryPath;
    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        upgradeHolder = UpgradeHolder.Instance;
        saveDirectoryPath = Application.persistentDataPath + "/Saves/";
        saveFilePath = saveDirectoryPath + fileName + ".json";
    }
    private void Start() {
        if (File.Exists(saveFilePath)){
            LoadFromFile();
        }
        else {
            CreateSaveFile();
        }
    }
    private void LoadFromFile()
    {
        try
        {
            string saveDataJson = File.ReadAllText(saveFilePath);
            savedStats = JsonUtility.FromJson<SavedStats>(saveDataJson);
            
            GameManager.Instance.Coins = savedStats.Coins;
            upgradeHolder.AddFromSave(savedStats.UpgradeLevels, savedStats.SkinsUnlocked, savedStats.EquippedBoat);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    private void CreateSaveFile()
    {
        try
        {
            if (!Directory.Exists(saveDirectoryPath))
            {
                Directory.CreateDirectory(saveDirectoryPath);
            }
            savedStats = new SavedStats
            {
                Coins = 0,
                UpgradeLevels = new List<UpgradeLevel>(),
                SkinsUnlocked = new List<BoatSkin>()
            };
            savedStats.UpgradeLevels.Add(new UpgradeLevel(UpgradeType.Magnet));
            savedStats.UpgradeLevels.Add(new UpgradeLevel(UpgradeType.Paddle));
            savedStats.UpgradeLevels.Add(new UpgradeLevel(UpgradeType.Armor));
            upgradeHolder.AddFromSave(savedStats.UpgradeLevels, savedStats.SkinsUnlocked, null);
            SaveToFile();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SaveUpgrades(List<UpgradeLevel> list)
    {
        savedStats.Coins = GameManager.Instance.Coins;
        savedStats.UpgradeLevels = list;
        SaveToFile();
    }
    
    public void SaveSkins(List<BoatSkin> list)
    {
        savedStats.Coins = GameManager.Instance.Coins;
        savedStats.SkinsUnlocked = list;
        SaveToFile();
    }

    public void SaveEquippedSkin(BoatSkin boatSkin)
    {
        savedStats.EquippedBoat = boatSkin;
        SaveToFile();
    }
    
    public void LevelSaved(int level, int coins)
    {
        savedStats.Coins = coins;
        SaveToFile();
    }

    public void SaveToFile()
    {
        try
        {
            string saveDataJson = JsonUtility.ToJson(savedStats);
            File.WriteAllText(saveFilePath, saveDataJson);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
