using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    public static GameManager Instance;
    
    [SerializeField] private List<GameData> gameDatas;
    [SerializeField] private LevelDataList levelDataList;
    [SerializeField] private float gameEndDragScale;
    [SerializeField] private float coinsPerSecond;
    [SerializeField] private int countDownTime;
    private LevelData[] levels;

    private const string UI = "UIScene";
    private readonly List<Interactable> interactableObjects = new();

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
            UIManager.Instance.UICameraSetActive(true);
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
        // Main menu and playing are the only two possible starting GameStates.
        switch(CurrentGameState)
        {
            case GameState.MainMenu:
                ReturnToMenu();
                UIManager.Instance.ShowMainMenuPanel();
                break;
            case GameState.Playing:
                UIManager.Instance.StartGame();
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
                    UIManager.Instance.PauseButtonSetActive(true);
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
        UIManager.Instance.ShowPausePanel();
        Time.timeScale = 0;
        AudioManager.Instance.StopRiverAudio();
    }
    public void LoadNextLevel()
    {
        AudioManager.Instance.PlayGameplayMusic();
        UIManager.Instance.loadingScreenPanel.MakeOpaque();
        int index = currentLevelIndex+1;
        if (levels.Length <= index)
        {
            ReturnToMenu();
            UIManager.Instance.ShowMainMenuPanel();
            return;
        }
        LoadLevel(index, () => StartCountdown());
    }
    public void LoadLevelInMenu(int index)
    {
        UIManager.Instance.menuBackgroundPanel.MakeOpaque();
        LoadLevel(index, () => {});
    }
    public void LoadLevel(int index, Action postLoadAction)
    {
        if (index == currentLevelIndex)
        {
            UIManager.Instance.menuBackgroundPanel.FadeOut();
            UIManager.Instance.loadingScreenPanel.FadeOut();
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
        UIManager.Instance.menuBackgroundPanel.FadeOut();
        UIManager.Instance.loadingScreenPanel.FadeOut();
        postLoadAction();
    }

    public void StartCountdown(bool tutorialIsOver = false)
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlayRiverAudio();
        AudioManager.Instance.PlayGameplayMusic();
        UIManager.Instance.WrongWayPanelSetActive(false);
        if (currentLevelIndex == 0 && !tutorialIsOver)
        {
            CurrentGameState = GameState.Tutorial;
            UIManager.Instance.StartTutorial();
            return;
        }
        UIManager.Instance.PauseButtonSetActive(false);
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
        UIManager.Instance.WrongWayPanelSetActive(false);
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
        AudioManager.Instance.StopRiverAudio();
        AudioManager.Instance.PlayMenuMusic();
        levelTimer.Reset();
        Coins = levelStartCoins;
        ResetLevel();
    }
    public void EndGame()
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
        
        UIManager.Instance.ShowGameEndPanel();
        AudioManager.Instance.PlayMenuMusic();
        SaveManager.Instance.Save();
    }
    public void FailGame()
    {
        CurrentGameState = GameState.GameOver;
        Time.timeScale = 0;
        AudioManager.Instance.StopRiverAudio();
        Coins = levelStartCoins;
        UIManager.Instance.ShowGameOverPanel();
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