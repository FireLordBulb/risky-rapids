using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Rower[] rowers;
    [Space] 
    [SerializeField] private ParticleSystem leftSplash;
    [SerializeField] private ParticleSystem rightSplash;
    [SerializeField] private ParticleSystem speedParticle;
    
    private PlayerHealth playerHealth;
    public int ActiveSpeedBoosts { get; set; }
    private void Awake()
    {
        InitializeHealth();
    }
    private void Start()
    {
        InitializeHealth();
        playerHealth.ReplenishHealth();
        GameManager.Instance.InitializePlayer(this);
        InitializePlayerModel();
    }
    private void InitializeHealth()
    {
        if (GameManager.Instance == null)
        {
            return;
        }
        SO_GameData currentGameData = GameManager.Instance.CurrentGameData;
        playerHealth = new PlayerHealth(currentGameData.MaxHealth, currentGameData.Armor);
    }
    public void InitializePlayerModel()
    {
        CharacterMesh hairStyle = (CharacterMesh)PlayerPrefs.GetInt("PlayerOneHair");
        CharacterColor hairColor = (CharacterColor)PlayerPrefs.GetInt("PlayerOneColor");
        foreach (Rower rower in rowers)
        {
            rower.SetActiveHairStyle(hairStyle);
            Material material = playerData.GetChosenHairStyleMaterials((int)hairStyle)[(int)hairColor];
            rower.SetMaterial(material);
            
            hairStyle = (CharacterMesh)PlayerPrefs.GetInt("PlayerTwoHair");
            hairColor = (CharacterColor)PlayerPrefs.GetInt("PlayerTwoColor");
        }
        
        rightSplash.Stop();
        leftSplash.Stop();
    }
    public void PlaySplashVFX(bool leftOar, bool rightOar)
    {
        if (leftOar)
        {
            leftSplash.Play(true); 
        }
        if (rightOar)
        {
            rightSplash.Play(true);
        }
    }
    public void StopSplashVFX()
    {
        rightSplash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        leftSplash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    public void DecreaseHealth(float damage)
    {
        playerHealth.TakeDamage(damage);
    }

    public void RestoreHealth()
    {
        if (playerHealth != null)
        {
            playerHealth.ReplenishHealth();
        }
    }

    public void StartSpeedVFX()
    {
        speedParticle.Play();
    }
    public void StopSpeedVFX(bool stopIsInstant)
    {
        if (stopIsInstant)
        {
            speedParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        } else
        {
            speedParticle.Stop();
        }
    }
    public void UpgradePlayerArmor()
    {
        playerHealth.UpgradeArmor();
    }
}
