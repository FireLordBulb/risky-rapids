using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private List<TextMeshProUGUI> coinTexts;
    [Space]
    [SerializeField] private TextMeshProUGUI endTime;
    [SerializeField] private TextMeshProUGUI endCoinsFromTime;
    [SerializeField] private TextMeshProUGUI endCoinsCollected;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject);
            return;
        } 
        Instance = this;
    }
    public void UpdateEndPanelText(string time, int coinsFromTime, int coinsCollected)
    {
        endTime.text = $"TIME : {time}";
        endCoinsFromTime.text = $"COINS FROM TIME : +{coinsFromTime}";
        endCoinsCollected.text = $"COINS COLLECTED : +{coinsCollected}";
    }
}
