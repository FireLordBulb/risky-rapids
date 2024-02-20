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
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject wrongWayPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject shopPanel;
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
        ToggleUICamera(false);
    }
    public void ToggleUICamera(bool toggle)
    {
        uiCamera.gameObject.SetActive(toggle);
    }
    public void LoadSpecificLevel(int index)
    {
        MakeActivePanel(characterSelectionPanel);
        ToggleMenuBackground(true);
        GameManager.Instance.LoadLevel(index);
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

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        MakeActivePanel(hudPanel);
    }
    public void OpenShopFromGame()
    {
        GameManager.Instance.ReturnToMenu();
        MakeActivePanel(shopPanel);
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
    public void ShowEndScreen()
    {
        MakeActivePanel(gameEndPanel);
    }
    public void ShowGameOverScreen()
    {
        MakeActivePanel(gameOverPanel);
    }
    public void RestartGame()
    {
        MakeActivePanel(hudPanel);
        GameManager.Instance.RestartGame();
    }
    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMenu();
        MakeActivePanel(mainMenuPanel);
    }
    public void LoadNextScene()
    {
        MakeActivePanel(hudPanel);
        GameManager.Instance.LoadNextLevel();
    }
    public void ActivatePausePanel()
    {
        MakeActivePanel(pausePanel);
    }
    public void StartTutorial()
    {
        tutorialPanel1.SetActive(true);
        TogglePauseButton(false);
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
    public void MakeActivePanel(GameObject panel)
    {
        activePanel.SetActive(false);
        activePanel = panel;
        activePanel.SetActive(true);
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
    public void UpdateHealthText(float health)
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
}
