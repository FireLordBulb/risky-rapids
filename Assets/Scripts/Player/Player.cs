using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    [SerializeField] private List<SkinnedMeshRenderer> bodyModels = new List<SkinnedMeshRenderer>();
    [SerializeField] private List<GameObject> hairStyles = new List<GameObject>();
    [SerializeField] private List<GameObject> hairStylesRowerTwo = new List<GameObject>();
    [Space] 
    [SerializeField] private ParticleSystem leftSplash;
    [SerializeField] private ParticleSystem rightSplash;
    [SerializeField] private ParticleSystem speedParticle;
    private float currentSpeedDuration;
    
    private PlayerHealth playerHealth;
    private void Start()
    {
        playerHealth = new PlayerHealth(GameManager.Instance.GetCurrentGameData().MaxHealth, GameManager.Instance.GetCurrentGameData().Armor);
        GameManager.Instance.InitializePlayer(this);
    }
    
    public void InitalizePlayerModel()
    {
        int hairStylePlayerOne = PlayerPrefs.GetInt("PlayerOneHair");
        int hairStylePlayerTwo = PlayerPrefs.GetInt("PlayerTwoHair");
        
        int hairColorPlayerOne = PlayerPrefs.GetInt("PlayerOneColor");
        int hairColorPlayerTwo = PlayerPrefs.GetInt("PlayerTwoColor");

        for (int i = 0; i < hairStyles.Count; i++)
        {
            hairStyles[i].SetActive(false);
            hairStylesRowerTwo[i].SetActive(false);
        }
        
        hairStyles[hairStylePlayerOne].SetActive(true);
        hairStylesRowerTwo[hairStylePlayerTwo].SetActive(true);
        
        var playerOneMaterials = playerData.GetChosenHairStyleMaterials(hairStylePlayerOne);
        hairStyles[hairStylePlayerOne].GetComponent<MeshRenderer>().material = playerOneMaterials[hairColorPlayerOne];
        
        var playerTwoMaterials = playerData.GetChosenHairStyleMaterials(hairStylePlayerTwo);
        hairStylesRowerTwo[hairStylePlayerTwo].GetComponent<MeshRenderer>().material = playerTwoMaterials[hairColorPlayerTwo];
        
        bodyModels[0].material = playerOneMaterials[hairColorPlayerOne];
        bodyModels[1].material = playerTwoMaterials[hairColorPlayerTwo];
        
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
        playerHealth.OnDamageTaken?.Invoke(damage);
    }

    public void RestoreHealth()
    {
        if (playerHealth != null)
        {
            playerHealth.OnReplenishHealth?.Invoke();
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
