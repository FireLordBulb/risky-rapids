using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentTimeText;
    private const int CentiPerWhole = 100;
    private const int SecondsPerMinute = 60;
    private const int SmallestTwoDigitInt = 10;
    private float currentTime;
    public float CurrentTime => currentTime;
    public string CurrentTimeString => currentTimeText.text;
    public void Reset()
    {
        currentTime = 0;
        DisplayTimeInSeconds();
    }
    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameStates.Playing)
        {
            return;
        }
        currentTime += Time.deltaTime;
        DisplayTimeInSeconds();
    }
    public void DisplayTimeInSeconds()
    {
        int centiSeconds = (int)(currentTime%1 * CentiPerWhole);
        int seconds = (int)currentTime%SecondsPerMinute;
        int minutes = (int)(currentTime/SecondsPerMinute);
        currentTimeText.text = $"{minutes}:{MakeTwoDigit(seconds)}.{MakeTwoDigit(centiSeconds)}";    
    }
    private static string MakeTwoDigit(int number)
    {
        return number < SmallestTwoDigitInt ? $"0{number}" : $"{number}";
    }
}