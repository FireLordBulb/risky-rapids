using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance;
    
    [SerializeField] private List<GameData> gameDatas;
    [SerializeField] private LevelDataList levelDataList;
    [SerializeField] private UIObjectLinker uiCameraLinker;
    [SerializeField] private UIObjectLinker mainMenuLinker;
    [SerializeField] private UIObjectLinker hudLinker;
    [SerializeField] private UIObjectLinker pauseMenuLinker;
    [SerializeField] private UIObjectLinker gameOverLinker;
    [SerializeField] private UIObjectLinker levelCompleteLinker;
    [SerializeField] private UIObjectLinker wrongWayLinker;
    [SerializeField] private UIObjectLinker pauseButtonLinker;
    [SerializeField] private UIObjectLinker tutorialLinker;
    [SerializeField] private UIObjectLinker menuBackgroundLinker;
    [SerializeField] private UIObjectLinker loadingScreenLinker;

    [SerializeField] private float gameEndDragScale;
    [SerializeField] private float coinsPerSecond;
    [SerializeField] private int countDownTime;
    private LevelData[] levels;

    private const string UI = "UIScene";
    private readonly List<Interactable> interactableObjects = new();

    private Tutorial tutorial;
    private LoadingScreenFade menuBackground;
    private LoadingScreenFade loadingScreen;
    
    private Vector3 playerSpawnPosition;
    private Quaternion playerSpawnRotation;
    private Player player;
    private BoatPhysics boatPhysics;
    private PlayerInput playerInput;
    private LevelTimer levelTimer;

    private LevelData currentLevel;
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
            ShopItemHolder.RefreshShopUI();
        }
    }
    public GameState CurrentGameState
    {
        get;
        private set;
    }
    public bool IsLoadingLevel
    {
        get;
        private set;
    }
    public GameData CurrentGameData
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
        CurrentGameData = gameDatas[0];
        levels = levelDataList.levels;
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levels.Length; i++)
        {
            LevelData level = levels[i];
            if (level.name.Equals(currentSceneName))
            {
                currentLevel = level;
                currentLevelIndex = i;
                break;
            }
        }
        if (currentSceneName.Equals(UI))
        {
            CurrentGameState = GameState.MainMenu;
            LoadLevel(0, () => {});
        } else
        { 
            CurrentGameState = GameState.Playing;
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
        tutorial = tutorialLinker.GameObject.GetComponent<Tutorial>();
        menuBackground = menuBackgroundLinker.GameObject.GetComponent<LoadingScreenFade>();
        loadingScreen = loadingScreenLinker.GameObject.GetComponent<LoadingScreenFade>();
        // Main menu and playing are the only two possible starting GameStates.
        switch(CurrentGameState)
        {
            case GameState.MainMenu:
                ReturnToMenu();
                break;
            case GameState.Playing:
                uiCameraLinker.GameObject.SetActive(false);
                ActivePanelSwitcher.SwitchTo(hudLinker);
                StartCountdown();
                break;
            case GameState.Paused:
            case GameState.EndGame:
            case GameState.GameOver:
            case GameState.CountDown:
            case GameState.Tutorial:
            default:
                Debug.LogError($"Invalid game start GameState: {CurrentGameState.ToString()}!!!");
                throw new ArgumentOutOfRangeException();
        }
    }
    public void SetStartCoins(int amount)
    {
        Coins = amount;
        levelStartCoins = amount;
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
                if (CurrentGameState == GameState.CountDown)
                {
                    pauseButtonLinker.GameObject.SetActive(true);
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
    public void CacheInteractable(Interactable interactableObject)
    {
        interactableObjects.Add(interactableObject);
    }
    public void PauseGame()
    {
        CurrentGameState = GameState.Paused;
        ActivePanelSwitcher.SwitchTo(pauseMenuLinker);
        Time.timeScale = 0;
        AudioManager.Instance.StopRiverAudio();
    }
    public void LoadNextLevel()
    {
        AudioManager.Instance.PlayGameplayMusic();
        int index = currentLevelIndex+1;
        if (levels.Length <= index)
        {
            ReturnToMenu();
            return;
        }
        loadingScreen.MakeOpaque();
        LoadLevel(index, () => StartCountdown());
    }
    public void LoadLevelInMenu(int index)
    {
        menuBackground.MakeOpaque();
        LoadLevel(index, () => {});
    }
    public void LoadLevel(int index, Action postLoadAction)
    {
        if (index == currentLevelIndex)
        {
            menuBackground.FadeOut();
            loadingScreen.FadeOut();
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
        IsLoadingLevel = true;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentLevel.name, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        IsLoadingLevel = false;
        Scene currentLevelScene = SceneManager.GetSceneByName(currentLevel.name);
        SceneManager.SetActiveScene(currentLevelScene);
        foreach (TerrainCollider terrainCollider in FindObjectsOfType<TerrainCollider>())
        {
            terrainCollider.enabled = false;
        }
        menuBackground.FadeOut();
        loadingScreen.FadeOut();
        postLoadAction();
    }

    public void StartCountdown(bool tutorialIsOver = false)
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlayRiverAudio();
        AudioManager.Instance.PlayGameplayMusic();
        WrongWayUISetActive(false);
        pauseButtonLinker.GameObject.SetActive(false);
        if (currentLevelIndex == 0 && !tutorialIsOver)
        {
            CurrentGameState = GameState.Tutorial;
            tutorial.StartTutorial();
            return;
        }
        CurrentGameState = GameState.CountDown;
        countDownLeft = countDownTime;
    }
    public void StartGame()
    {
        CurrentGameState = GameState.Playing;
        Time.timeScale = 1;
        AudioManager.Instance.PlayGameplayMusic();
        AudioManager.Instance.PlayRiverAudio();
        levelTimer.Reset();
        levelStartCoins = Coins;
        WrongWayUISetActive(false);
    }
    public void ResumeGame()
    {
        CurrentGameState = GameState.Playing;
        Time.timeScale = 1;
        AudioManager.Instance.PlayRiverAudio();
    }
    public void RestartGame()
    {
        ResetLevel();
        Coins = levelStartCoins;
        StartGame();
    }
    public void ReturnToMenu()
    {
        CurrentGameState = GameState.MainMenu;
        Time.timeScale = 1;
        ActivePanelSwitcher.SwitchTo(mainMenuLinker);
        AudioManager.Instance.StopRiverAudio();
        AudioManager.Instance.PlayMenuMusic();
        levelTimer.Reset();
        Coins = levelStartCoins;
        ResetLevel();
    }
    public void CompleteLevel()
    {
        CurrentGameState = GameState.EndGame;
        boatPhysics.ScaleLinearDrag(gameEndDragScale);
        
        int coinsCollected = Coins-levelStartCoins;
        float timeTaken = levelTimer.CurrentTime;
        float timeDifference = currentLevel.baseTime - timeTaken;
        int coinsFromTime = Math.Max((int)(coinsPerSecond*timeDifference), 0);
        Coins += coinsFromTime;
        levelStartCoins = Coins;
        UIManager.Instance.UpdateEndPanelText(levelTimer.CurrentTimeString, coinsFromTime, coinsCollected);
        levelTimer.Reset();
        
        ActivePanelSwitcher.SwitchTo(levelCompleteLinker);
        AudioManager.Instance.PlayMenuMusic();
        SaveManager.Instance.Save();
    }
    public void FailGame()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0;
        AudioManager.Instance.StopRiverAudio();
        Coins = levelStartCoins;
        ActivePanelSwitcher.SwitchTo(gameOverLinker);
    }

    public void WrongWayUISetActive(bool isActive)
    {
        wrongWayLinker.GameObject.SetActive(isActive);
    }
    private void ResetLevel()
    {
        ResetPlayer();
        ResetInteractableObjects();
    }
    private void ResetPlayer()
    {
        if (!boatPhysics) return;
        boatPhysics.ResetTo(playerSpawnPosition, playerSpawnRotation);
        player.RestoreHealth();
        playerInput.SetIdle();
    }
    private void ResetInteractableObjects()
    {
        foreach (Interactable interactable in interactableObjects)
        {
            interactable.ResetInteractable();
        }
    }
}