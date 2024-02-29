using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
public class SaveData
{
    private const int RowerCount = 2;
    public int Coins;
    // ReSharper disable FieldCanBeMadeReadOnly.Global
    // Don't make readonly! It breaks the JSON reading and writing.
    public UpgradeLevel[] UpgradeLevels;
    public List<BoatSkin> OwnedSkins;
    public BoatSkin EquippedSkin;
    public CharacterAppearance[] CharacterAppearances;
    public SaveData()
    {
        Coins = 0;
        UpgradeLevels = new UpgradeLevel[]{new(UpgradeType.Magnet), new(UpgradeType.Paddle), new(UpgradeType.Armor)};
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
    public RowerColor color;
    public RowerMesh mesh;
    public CharacterAppearance(int rowerIndex)
    {
        color = (RowerColor)rowerIndex;
        mesh = (RowerMesh)rowerIndex;
    }
}
[Serializable]
public class UpgradeLevel
{
    public UpgradeType upgradeType;
    public int level;

    public UpgradeLevel(UpgradeType type)
    {
        upgradeType = type;
        level = 0;
    }
    public void IncreaseLevel()
    {
        level++;
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
    private void Start()
    {
        try
        {
            if (File.Exists(saveFilePath)){
                LoadFromFile();
            }
            else {
                CreateSaveFile();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        GameManager.Instance.SetStartCoins(saveData.Coins);
        UpgradeHolder.Instance.AddFromSave(saveData);
    }
    private void LoadFromFile()
    {
        string saveDataJson = File.ReadAllText(saveFilePath);
        saveData = JsonUtility.FromJson<SaveData>(saveDataJson);
    }
    private void CreateSaveFile()
    {
        if (!Directory.Exists(saveDirectoryPath))
        {
            Directory.CreateDirectory(saveDirectoryPath);
        }
        SaveToFile();
    }

    public void Save()
    {
        saveData.Coins = GameManager.Instance.Coins;
        saveData.EquippedSkin = UpgradeHolder.Instance.ActiveBoatSkin;
        SaveToFile();
    }
    private void SaveToFile()
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
