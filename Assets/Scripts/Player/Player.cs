using UnityEngine;

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
        foreach (Rower rower in rowers)
        {
            rower.Initialize(playerData);
        }
    }
    private void Start()
    {
        InitializeHealth();
        playerHealth.ReplenishHealth();
        GameManager.Instance.InitializePlayer(this);
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
    public void InitializeModel(CharacterAppearance[] characterAppearances)
    {
        for (int i = 0; i < rowers.Length; i++)
        {
            var rower = rowers[i];
            var characterAppearance = characterAppearances[i];
            rower.SetColor(characterAppearance.color);
            rower.SetMesh(characterAppearance.mesh);
        }

        rightSplash.Stop();
        leftSplash.Stop();
    }
    public void SetRowerColor(int rowerIndex, CharacterColor color)
    {
        rowers[rowerIndex].SetColor(color);
    }
    public void SetRowerMesh(int rowerIndex, CharacterMesh mesh)
    {
        rowers[rowerIndex].SetMesh(mesh);
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
