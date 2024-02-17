using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance;
    
    [SerializeField] private List<SO_GameData> gameDatas;
    [SerializeField] private SO_LevelDataList levelDataList;
    [SerializeField] private float coinsPerSecond;
    [SerializeField] private int countDownTime;
    private SO_LevelData[] levels;

    private const string UI = "UIScene";
    private SO_GameData currentGameData;
    private readonly List<IInteractable> interactableObjects = new();

    private Vector3 playerSpawnPosition;
    private Quaternion playerSpawnRotation;
    private Player player;
    private BoatPhysics boatPhysics;
    private PlayerInput playerInput;
    private LevelTimer levelTimer;
    private UpgradeHolder upgradeHolder;

    private SO_LevelData currentLevel;
    private int currentLevelIndex = -1;

    private float countDownLeft;

    private int levelStartCoins;
    private int coins;
    public int Coins
    {
        get => coins;
        set
        {
            coins = value;
            UIManager.Instance.UpdateCoinTexts();
        }
    }
    public GameStates CurrentGameState
    {
        get;
        private set;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        } 
        Instance = this; 
#if UNITY_STANDALONE_WIN
        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
#endif
        currentGameData = gameDatas[0];
        levels = levelDataList.levels;
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levels.Length; i++)
        {
            SO_LevelData level = levels[i];
            if (level.name.Equals(currentSceneName))
            {
                currentLevel = level;
                currentLevelIndex = i;
                break;
            }
        }
        if (currentSceneName.Equals(UI))
        {
            CurrentGameState = GameStates.MainMenu;
            LoadLevel(0);
        } else { 
            CurrentGameState = GameStates.Playing;
            SceneManager.LoadScene(UI, LoadSceneMode.Additive);
            if (currentLevel == null)
            {
                currentLevelIndex = -1;
            }
        }
    }
    private void Start()
    {
        levelTimer = FindObjectOfType<LevelTimer>(true);
        upgradeHolder = FindObjectOfType<UpgradeHolder>();
        // Main menu and playing are the only two possible starting GameStates.
        switch(CurrentGameState)
        {
            case GameStates.MainMenu:
                UIManager.Instance.ReturnToMainMenu();
                break;
            case GameStates.Playing:
                UIManager.Instance.StartGame();
                break;
            case GameStates.Paused:
            case GameStates.EndGame:
            case GameStates.GameOver:
            case GameStates.CountDown:
            case GameStates.Tutorial:
            default:
                Debug.LogError($"Invalid game start GameState: {CurrentGameState.ToString()}!!!");
                throw new ArgumentOutOfRangeException();
        }
    }
    private void Update()
    {
        if (0 < countDownLeft)
        {
            countDownLeft -= Time.deltaTime;
            string countDownText = $"{(int)countDownLeft}";
            if (countDownText.Equals("0"))
            {
                countDownText = "GO!";
                if (CurrentGameState == GameStates.CountDown)
                {
                    UIManager.Instance.TogglePauseButton(true);
                    StartGame();
                }
            }
            UIManager.Instance.UpdateCountDownText(countDownText);
        } else
        {
            UIManager.Instance.UpdateCountDownText("");
        }
    }
    public void InitializePlayer(Player newPlayer)
    {
        player = newPlayer;
        playerSpawnPosition = player.transform.position;
        playerSpawnRotation = player.transform.rotation;
        boatPhysics = player.GetComponent<BoatPhysics>();
        playerInput = player.GetComponent<PlayerInput>();
    }
    public void CacheInteractable(IInteractable interactableObject)
    {
        interactableObjects.Add(interactableObject);
    }
    public void PauseGame()
    {
        CurrentGameState = GameStates.Paused;
        UIManager.Instance.OnGamePaused?.Invoke();
        Time.timeScale = 0;
    }
    public void LoadNextLevel()
    {
        AudioManager.Instance.StopMenuAudio();
        AudioManager.Instance.PlayBackgroundAudio();
        UIManager.Instance.ToggleLoadingScreen(true);
        int index = currentLevelIndex+1;
        if (levels.Length <= index)
        {
            UIManager.Instance.ReturnToMainMenu();
            return;
        }
        LoadLevel(index, () => StartCountdown());
    }
    public void LoadLevel(int index)
    {
        LoadLevel(index, () => { });
    }
    public void LoadLevel(int index, Action postLoadAction)
    {
        if (index == currentLevelIndex)
        {
            return;
        }
        if (currentLevel != null)
        {
            SceneManager.UnloadSceneAsync(currentLevel.name);
            interactableObjects.Clear();
        }
        currentLevelIndex = index;
        currentLevel = levels[currentLevelIndex];
        StartCoroutine(LoadLevelCoroutine(postLoadAction));
    }
    IEnumerator LoadLevelCoroutine(Action postLoadAction)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentLevel.name, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Scene currentLevelScene = SceneManager.GetSceneByName(currentLevel.name);
        SceneManager.SetActiveScene(currentLevelScene);
        foreach (TerrainCollider terrainCollider in FindObjectsOfType<TerrainCollider>())
        {
            terrainCollider.enabled = false;
        }
        player.InitalizePlayerModel();
        upgradeHolder.FixUpgrades();
        upgradeHolder.ApplyCurrentBoatSkin();
        UIManager.Instance.ToggleMenuBackground(false);
        UIManager.Instance.ToggleLoadingScreen(false);
        postLoadAction();
    }

    public void StartCountdown(bool tutorialIsOver = false)
    {
        Time.timeScale = 1;
        player.InitalizePlayerModel();
        if (currentLevelIndex == 0 && !tutorialIsOver)
        {
            CurrentGameState = GameStates.Tutorial;
            UIManager.Instance.StartTutorial();
            return;
        }
        UIManager.Instance.TogglePauseButton(false);
        CurrentGameState = GameStates.CountDown;
        countDownLeft = countDownTime;
    }
    public void StartGame()
    {
        CurrentGameState = GameStates.Playing;
        Time.timeScale = 1;
        levelStartCoins = Coins;
    }
    public void RestartGame()
    {
        ResetLevel();
        Coins = levelStartCoins;
        StartGame();
    }
    public void ReturnToMenu()
    {
        CurrentGameState = GameStates.MainMenu;
        Time.timeScale = 1;
        ResetLevel();
    }

    private void ResetCoins()
    {
        Coins = 0;
    }
    
    public void EndGame()
    {
        CurrentGameState = GameStates.EndGame;

        int coinsCollected = Coins-levelStartCoins;
        float timeTaken = levelTimer.CurrentTime;
        float timeDifference = currentLevel.baseTime - timeTaken;
        int coinsFromTime = Math.Max((int)(coinsPerSecond*timeDifference), 0);
        Coins += coinsFromTime;
        levelStartCoins = Coins;
        UIManager.Instance.SetEndPanelValues(levelTimer.CurrentTimeString, coinsFromTime, coinsCollected);
        levelTimer.Reset();

        UIManager.Instance.OnGameEnded?.Invoke();
        SaveManager.Instance.SaveCoins(Coins);
    }
    public void FailGame()
    {
        CurrentGameState = GameStates.GameOver;
        Time.timeScale = 0;
        Coins = levelStartCoins;
        UIManager.Instance.ShowGameOverScreen();
    }
    private void ResetLevel()
    {
        ResetPlayer();
        ResetInteractableObjects();
    }
    private void ResetPlayer()
    {
        if (!boatPhysics) return;
        Rigidbody rb = boatPhysics.Rigidbody;
        boatPhysics.transform.position = playerSpawnPosition;
        rb.position = playerSpawnPosition;
        boatPhysics.transform.rotation = playerSpawnRotation;
        rb.rotation = playerSpawnRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Resets force and torque to zero.
        rb.AddForce(-rb.GetAccumulatedForce());
        rb.AddTorque(-rb.GetAccumulatedTorque());
        
        player.RestoreHealth();
        playerInput.SetIdle();
    }
    private void ResetInteractableObjects()
    {
        foreach (IInteractable interactable in interactableObjects)
        {
            interactable.Reset();
        }
    }
    public SO_GameData GetCurrentGameData()
    {
        return currentGameData;
    }
    
    private void OnSceneLoaded(Scene sceneToLoad, LoadSceneMode sceneLoadMode)
    {
        if(!UIManager.Instance) return;
        UIManager.Instance.OnHealthUpdated?.Invoke(gameDatas[0].MaxHealth);
    }

    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  
    }
}