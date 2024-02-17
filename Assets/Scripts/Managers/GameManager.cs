using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance;
    
    [SerializeField] private GameObject finishLinePrefab;
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
                CurrentGameState = GameStates.Playing;
                currentLevel = level;
                currentLevelIndex = i;
                break;
            }
        }
        if (currentSceneName.Equals(UI))
        {
            CurrentGameState = GameStates.MainMenu;
            LoadLevelWithoutStarting(0);
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
        InitializeCurrentLevel();
    }
    private void InitializeCurrentLevel()
    {
        if (FindObjectOfType<FinishLine>() == null)
        {
            Instantiate(finishLinePrefab, SplineManager.Instance.Positions[^1], Quaternion.LookRotation(SplineManager.Instance.Directions[^2]));
        }
        TerrainCollider[] terrainColliders = FindObjectsOfType<TerrainCollider>();
        foreach (TerrainCollider terrainCollider in terrainColliders)
        {
            terrainCollider.enabled = false;
        }
    }
    // Pain death, all hope is lost. Update method in GameManager is needed.
    private void FixedUpdate()
    {
        /*if (!SceneManager.GetSceneByName(currentLevel.name).isLoaded)
        {
            return;
        }
        if (SceneManager.GetSceneByName(currentLevel.name) != SceneManager.GetActiveScene())
        {
            MakeCurrentLevelActiveScene();
        }
        if (hasLoadedNewLevel)
        {
            hasLoadedNewLevel = false;
            MakeCurrentLevelActiveScene();
            InitializeCurrentLevel();
            player.InitalizePlayerModel();
            UIManager.Instance.UpdatePlayer();
        }*/
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
    public void InitalizePlayer(Player newPlayer)
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
        LoadLevel(currentLevelIndex+1);
    }
    public void LoadLevel(int index)
    {
        if (index < 0 || levels.Length <= index)
        {
            Debug.Log($"Level with index {index} doesn't exist. Returning to Main Menu.");
            UIManager.Instance.ReturnToMainMenu();
            return;
        }
        UIManager.Instance.ToggleLoadingScreen(true);
        AudioManager.Instance.StopMenuAudio();
        AudioManager.Instance.PlayBackgroundAudio();
        LoadLevelCore(index, true);
    }
    public void LoadLevelWithoutStarting(int index)
    {
        if (index < 0 || levels.Length <= index)
        {
            Debug.Log($"Level with index {index} doesn't exist. Returning to Main Menu.");
            UIManager.Instance.ReturnToMainMenu();
            return;
        }
        LoadLevelCore(index, false);
    }
    private void LoadLevelCore(int index, bool doStart)
    {
        if (index == currentLevelIndex)
        {
            return;
        }
        string previousLevelName = null;
        if (currentLevel != null)
        {
            previousLevelName = currentLevel.name;
        }
        interactableObjects.Clear();
        if (levelTimer != null)
        {
            levelTimer.Reset();
        }
        currentLevelIndex = index;
        currentLevel = levels[currentLevelIndex];
        string nextLevelName = currentLevel.name;
        if (previousLevelName != null && currentLevelIndex != -1)
        {
            SaveManager.Instance.LevelSaved(currentLevelIndex, Coins);
        }
        if (doStart)
        {
            StartCountdown();
        }
        if (previousLevelName != null)
        {
            StartCoroutine(UnloadAnAsyncScene(SceneManager.GetSceneByName(previousLevelName), nextLevelName));
            
        }
        StartCoroutine(LoadAnAsyncScene(nextLevelName));
    }
    IEnumerator UnloadAnAsyncScene(Scene scene, string nextLevelName)
    {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(scene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
    }
    IEnumerator LoadAnAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        MakeCurrentLevelActiveScene();
        InitializeCurrentLevel();
        player.InitalizePlayerModel();
        UIManager.Instance.UpdatePlayer();
        UIManager.Instance.ToggleLoadingScreen(false);
    }
    private void MakeCurrentLevelActiveScene()
    {
        Scene currentLevelScene = SceneManager.GetSceneByName(currentLevel.name);
        SceneManager.SetActiveScene(currentLevelScene);
    }
    public void StartCountdown(bool tutorialIsOver = false)
    {
        if (SceneManager.GetSceneByName(currentLevel.name).isLoaded)
        {
            player.InitalizePlayerModel();
        }
        Time.timeScale = 1;
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
        ResetLevel();
        Time.timeScale = 1;
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
        UIManager.Instance.OnGameEnded?.Invoke();
        if (currentLevelIndex != -1)
        {
            SaveManager.Instance.LevelSaved(currentLevelIndex, Coins);
        }
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
        ResetPlayerPosition();
        ReEnableAllInteractables();
        levelTimer.Reset();
        
    }
    private void ResetPlayerPosition()
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
    private void ReEnableAllInteractables()
    {
        foreach (IInteractable interactable in interactableObjects)
        {
            interactable.Activate();
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