using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    [SerializeField] private List<SkinnedMeshRenderer> bodyModels = new();
    [SerializeField] private List<GameObject> hairStyles = new();
    [SerializeField] private List<GameObject> hairStylesRowerTwo = new();
    [Space] 
    [SerializeField] private ParticleSystem leftSplash;
    [SerializeField] private ParticleSystem rightSplash;
    [SerializeField] private ParticleSystem speedParticle;
    
    private PlayerHealth playerHealth;
    public int ActiveSpeedBoosts { get; set; }

    private void Start()
    {
        playerHealth = new PlayerHealth(GameManager.Instance.GetCurrentGameData().MaxHealth, GameManager.Instance.GetCurrentGameData().Armor);
        playerHealth.ReplenishHealth();
        GameManager.Instance.InitializePlayer(this);
    }
    
    public void InitializePlayerModel()
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
