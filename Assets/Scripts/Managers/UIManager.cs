using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject wrongWayPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject tutorialPanel1;
    [SerializeField] private GameObject tutorialPanel2;
    [SerializeField] private GameObject pauseButton;
    // An opaque panel to be behind the transparent menu panel during level loading.
    [SerializeField] private LoadingScreenFade menuBackgroundPanel;
    [SerializeField] private LoadingScreenFade loadingScreenPanel;
    
    [Space] [Space] 
    
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private List<TextMeshProUGUI> coinTexts;
    [SerializeField] private Camera uiCamera;
    [Space]
    [SerializeField] private TextMeshProUGUI endTime;
    [SerializeField] private TextMeshProUGUI endCoinsFromTime;
    [SerializeField] private TextMeshProUGUI endCoinsCollected;

    public Action<float> OnHealthUpdated;
    public Action OnGameEnded;
    public Action OnGameStart;
    public Action OnGamePaused;
    public Action OnGameResume;
    public Action OnGameReturnToMenu;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject);
            return;
        } 
        Instance = this;

        ToggleUICamera(false);
    }
    public void ToggleUICamera(bool toggle)
    {
        uiCamera.gameObject.SetActive(toggle);
    }
    public void LoadSpecificLevel(int index)
    {
        ToggleCharacterSelect(true);
        ToggleMenuBackground(true);
        GameManager.Instance.LoadLevel(index);
    }
    
    public void StartGame()
    {
        if (GameManager.Instance.IsLoadingLevel)
        {
            return;
        }
        OnGameStart?.Invoke();
        GameManager.Instance.StartCountdown();
        mainMenuPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
        hudPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        OnGameResume?.Invoke();
        GameManager.Instance.StartGame();
        pausePanel.SetActive(false);
        hudPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void OpenShopFromGame()
    {
        GameManager.Instance.ReturnToMenu();
        shopPanel.SetActive(true);
        hudPanel.SetActive(false);
        gameEndPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void ToggleLevelSelect(bool toggle)
    {
        levelSelectPanel.SetActive(toggle);
        mainMenuPanel.SetActive(!toggle);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void ToggleCharacterSelect(bool toggle)
    {
        characterSelectionPanel.SetActive(toggle);
        levelSelectPanel.SetActive(!toggle);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void ToggleShop(bool toggle)
    {
        shopPanel.SetActive(toggle);
        mainMenuPanel.SetActive(!toggle);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void ToggleCredits(bool toggle)
    {
        creditsPanel.SetActive(toggle);
        mainMenuPanel.SetActive(!toggle);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void TogglePauseButton(bool toggle)
    {
        pauseButton.SetActive(toggle);
    }
    public void ToggleMenuBackground(bool toggle)
    {
        if (toggle)
        {
            menuBackgroundPanel.MakeSolid();
        } else
        {
            menuBackgroundPanel.FadeOut();
        }
    }
    public void ToggleLoadingScreen(bool toggle)
    {
        if (toggle)
        {
            loadingScreenPanel.MakeSolid();
        } else
        {
            loadingScreenPanel.FadeOut();
        }
    }
    private void ShowEndScreen()
    {
        gameEndPanel.SetActive(true);
        hudPanel.SetActive(false);
        wrongWayPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        AudioManager.Instance.StopBackgroundAudio();
        AudioManager.Instance.PlayMenuAudio();
    }
    public void ShowGameOverScreen()
    {
        hudPanel.SetActive(false);
        wrongWayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

    }
    public void RestartGame()
    {
        hudPanel.SetActive(true);
        gameEndPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        if (GameManager.Instance.CurrentGameState == GameState.EndGame)
        {
            AudioManager.Instance.StopMenuAudio();
            AudioManager.Instance.PlayBackgroundAudio();
        }
        GameManager.Instance.RestartGame();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMenu();
        hudPanel.SetActive(false);
        wrongWayPanel.SetActive(false);
        gameEndPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        OnGameReturnToMenu?.Invoke();
    }

    public void LoadNextScene()
    {
        hudPanel.SetActive(true);
        gameEndPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        GameManager.Instance.LoadNextLevel();
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void ActivatePausePanel()
    {
        hudPanel.SetActive(false);
        wrongWayPanel.SetActive(false);
        pausePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void StartTutorial()
    {
        tutorialPanel1.SetActive(true);
        TogglePauseButton(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void ContinueTutorial()
    {
        tutorialPanel1.SetActive(false);
        tutorialPanel2.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void EndTutorial()
    {
        tutorialPanel2.SetActive(false);
        GameManager.Instance.StartCountdown(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void WrongWayPanelSetActive(bool isActive)
    {
        wrongWayPanel.SetActive(isActive);
    }

    private void UpdateHealthText(float health)
    {
        if (Instance == null) return;
        healthSlider.value = health;
    }
    public void UpdateCountDownText(string text)
    {
        countDownText.text = text;
    }
    public void UpdateCoinTexts()
    {
        foreach (TextMeshProUGUI coinText in coinTexts)
        {
            if (coinText == null) continue;
                
            coinText.text = GameManager.Instance.Coins.ToString();
        }
    }
    public void SetEndPanelValues(string time, int coinsFromTime, int coinsCollected)
    {
        endTime.text = $"TIME : {time}";
        endCoinsFromTime.text = $"COINS FROM TIME : +{coinsFromTime}";
        endCoinsCollected.text = $"COINS COLLECTED : +{coinsCollected}";
    }
    private void OnEnable()
    {
        OnGameEnded += ShowEndScreen;
        OnHealthUpdated += UpdateHealthText;
        OnGamePaused += ActivatePausePanel;
        
        OnGameEnded += UpdateCoinTexts;
    }

    private void OnDisable()
    {
        OnHealthUpdated -= UpdateHealthText;
        OnGameEnded -= ShowEndScreen;
        OnGamePaused -= ActivatePausePanel;
    }
}
