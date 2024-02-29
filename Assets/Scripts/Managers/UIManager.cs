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
    
    [SerializeField] private GameObject wrongWayPanel;
    [SerializeField] private GameObject mainMenuPanel;
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
    public void MakeActivePanel(GameObject panel)
    {
        activePanel.SetActive(false);
        activePanel = panel;
        activePanel.SetActive(true);
    }
}
