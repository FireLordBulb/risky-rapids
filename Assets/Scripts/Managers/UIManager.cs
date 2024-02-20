using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public LoadingScreenFade menuBackgroundPanel;
    public LoadingScreenFade loadingScreenPanel;
    
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject wrongWayPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private GameObject controlsTutorialPanel;
    [SerializeField] private GameObject goalTutorialPanel;
    [SerializeField] private GameObject pauseButton;
    
    [Space] [Space] 
    
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private List<TextMeshProUGUI> coinTexts;
    [SerializeField] private Camera uiCamera;
    [Space]
    [SerializeField] private TextMeshProUGUI endTime;
    [SerializeField] private TextMeshProUGUI endCoinsFromTime;
    [SerializeField] private TextMeshProUGUI endCoinsCollected;

    private GameObject activePanel;
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject);
            return;
        } 
        Instance = this;

        activePanel = mainMenuPanel;
        UICameraSetActive(false);
    }
    public void StartGame()
    {
        if (GameManager.Instance.IsLoadingLevel)
        {
            return;
        }
        GameManager.Instance.StartCountdown();
        MakeActivePanel(hudPanel);
    }
    public void UpdateEndPanelText(string time, int coinsFromTime, int coinsCollected)
    {
        endTime.text = $"TIME : {time}";
        endCoinsFromTime.text = $"COINS FROM TIME : +{coinsFromTime}";
        endCoinsCollected.text = $"COINS COLLECTED : +{coinsCollected}";
    }
    public void UpdateCoinTexts()
    {
        foreach (TextMeshProUGUI coinText in coinTexts)
        {
            coinText.text = GameManager.Instance.Coins.ToString();
        }
    }
    public void UpdateCountDownText(string text)
    {
        countDownText.text = text;
    }
    public void UpdateHealthSlider(float health)
    {
        healthSlider.value = health;
    }
    public void UICameraSetActive(bool isActive)
    {
        uiCamera.gameObject.SetActive(isActive);
    }
    public void WrongWayPanelSetActive(bool isActive)
    {
        wrongWayPanel.SetActive(isActive);
    }
    public void PauseButtonSetActive(bool isActive)
    {
        pauseButton.SetActive(isActive);
    }
    public void ShowMainMenuPanel()
    {
        MakeActivePanel(mainMenuPanel);
    }
    public void ShowGameEndPanel()
    {
        MakeActivePanel(gameEndPanel);
    }
    public void ShowGameOverPanel()
    {
        MakeActivePanel(gameOverPanel);
    }
    public void ShowPausePanel()
    {
        MakeActivePanel(pausePanel);
    }
    public void MakeActivePanel(GameObject panel)
    {
        activePanel.SetActive(false);
        activePanel = panel;
        activePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void StartTutorial()
    {
        controlsTutorialPanel.SetActive(true);
        PauseButtonSetActive(false);
    }
    public void ContinueTutorial()
    {
        controlsTutorialPanel.SetActive(false);
        goalTutorialPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void EndTutorial()
    {
        GameManager.Instance.StartCountdown(true);
        goalTutorialPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    // GameManager method relays |>----------------------------------------------------------------------------------------
    public void GameManagerLoadLevel(int index)
    {
        GameManager.Instance.LoadLevelInMenu(index);
    }
    public void GameManagerLoadNextLevel()
    {
        GameManager.Instance.LoadNextLevel();
    }
    public void GameManagerReturnToMenu()
    {
        GameManager.Instance.ReturnToMenu();
    }
    public void GameManagerRestartGame()
    {
        GameManager.Instance.RestartGame();
    }
    public void GameManagerResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }
}
