using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavedStats
{
    // Name
    public string PlayerName;
    public int Coins;
    
    // Score and placements on levels
    public int CurrentSeasonLevel;
    
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
    [Header("Saving Data")]
    private SavedStats savedStats;
    private string saveFilePath;
    private UpgradeHolder upgradeHolder;
    private static readonly string keyword = "DaSbOaTgAmE";
    [SerializeField] private string fileName = "Save";
    [SerializeField] private bool makeDataEcrypted = true;
    
    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple Save Managers");
            Destroy(this);
        }
        /*savedStats = new SavedStats
        {
            PlayerName = " ",
            Coins = 0,
            CurrentSeasonLevel = 0,
            UpgradeLevels = new List<UpgradeLevel>(),
            SkinsUnlocked = new List<BoatSkin>()
        };*/
        upgradeHolder = UpgradeHolder.Instance;
        saveFilePath = Application.dataPath + "/Saves/" + fileName + ".json";
    }

    private void Start()
    {
        if (File.Exists(saveFilePath) == false)
        {
            CreateSaveFile();
        }
        else
        {
            LoadFromFile();
        }
    }

    public void CreateSaveFile()
    {
        try
        {
            if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            {
                Directory.CreateDirectory(Application.dataPath + "/Saves/");
            }
            savedStats = new SavedStats
            {
                PlayerName = " ",
                Coins = 0,
                CurrentSeasonLevel = 0,
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
        try
        {
            savedStats.Coins = GameManager.Instance.Coins;
            savedStats.UpgradeLevels = list;
            SaveToFile();
        } catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    public void SaveSkins(List<BoatSkin> list)
    {
        try
        {
            savedStats.Coins = GameManager.Instance.Coins;
            savedStats.SkinsUnlocked = list;
            SaveToFile();
        } catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SaveEquippedSkin(BoatSkin boatSkin)
    {
        try
        {
            savedStats.EquippedBoat = boatSkin;
            SaveToFile();
        } catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    public void LevelSaved(int level, int coins)
    {
        try
        {
            savedStats.CurrentSeasonLevel = level;
            savedStats.Coins = coins;
            SaveToFile();
        } catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SaveToFile()
    {
        try
        {
            string savePlayerData = JsonUtility.ToJson(savedStats);
            if (makeDataEcrypted == true)
            {
                savePlayerData = DecryptAndEncrypt(savePlayerData);
            }
            File.WriteAllText(saveFilePath, savePlayerData);
            Debug.Log("File saved");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    public void LoadFromFile()
    {
        try
        {
            string saveString = File.ReadAllText(saveFilePath);
            if (makeDataEcrypted)
            {
                savedStats = JsonUtility.FromJson<SavedStats>(DecryptAndEncrypt(saveString));
            }
            else
            {
                savedStats = JsonUtility.FromJson<SavedStats>(saveString);
            }
            GameManager.Instance.Coins = savedStats.Coins;
            upgradeHolder.AddFromSave(savedStats.UpgradeLevels, savedStats.SkinsUnlocked, savedStats.EquippedBoat);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private static string DecryptAndEncrypt(string data)
    {
        string result = "";
        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ keyword[i % keyword.Length]);
        }

        return result;
    }
}
