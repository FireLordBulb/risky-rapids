using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData
{
    private const int RowerCount = 2;
    public int Coins;
    public UpgradeLevel[] UpgradeLevels;
    public List<BoatSkin> OwnedSkins;
    public BoatSkin EquippedSkin;
    public CharacterAppearance[] CharacterAppearances;
    public SaveData()
    {
        Coins = 0;
        UpgradeLevels = new UpgradeLevel[] { new(UpgradeType.Magnet), new(UpgradeType.Paddle), new(UpgradeType.Armor) };
        OwnedSkins = new List<BoatSkin>();
        CharacterAppearances = new CharacterAppearance[RowerCount];
        for (int i = 0; i < CharacterAppearances.Length; i++)
        {
            CharacterAppearances[i] = new CharacterAppearance(i);
        }
    }
}

[Serializable]
public class CharacterAppearance
{
    public CharacterColor color;
    public CharacterMesh mesh;
    public CharacterAppearance(int rowerIndex)
    {
        color = (CharacterColor)rowerIndex;
        mesh = (CharacterMesh)rowerIndex;
    }
}
[Serializable]
public class UpgradeLevel
{
    public UpgradeType UpgradeType;
    public int Level;

    public UpgradeLevel(UpgradeType upgradeType)
    {
        UpgradeType = upgradeType;
        Level = 0;
    }

    public void IncreaseLevel()
    {
        Level++;
    }
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    [SerializeField] private string fileName = "Save";
    
    private SaveData saveData = new();
    private string saveDirectoryPath;
    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
            saveData = JsonUtility.FromJson<SaveData>(saveDataJson);
            
            GameManager.Instance.SetStartCoins(saveData.Coins);
            UpgradeHolder.Instance.AddFromSave(saveData);
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
            SaveToFile();
            UpgradeHolder.Instance.AddFromSave(saveData);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SaveUpgrades(UpgradeLevel[] levels)
    {
        saveData.Coins = GameManager.Instance.Coins;
        saveData.UpgradeLevels = levels;
        SaveToFile();
    }
    
    public void SaveSkins(List<BoatSkin> list)
    {
        saveData.Coins = GameManager.Instance.Coins;
        saveData.OwnedSkins = list;
        SaveToFile();
    }

    public void SaveEquippedSkin(BoatSkin boatSkin)
    {
        saveData.EquippedSkin = boatSkin;
        SaveToFile();
    }
    
    public void SaveCoins(int coins)
    {
        saveData.Coins = coins;
        SaveToFile();
    }

    public void SaveToFile()
    {
        try
        {
            string saveDataJson = JsonUtility.ToJson(saveData);
            File.WriteAllText(saveFilePath, saveDataJson);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
