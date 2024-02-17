using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CoinCollection : MonoBehaviour
{
    private int CoinsCollected;
    
    private LevelTimer levelTimer;
    
    [SerializeField][Tooltip("if this variable is 2 and you finish with 5 sec left on countdown timer then you will get 5X2 = 10 coins")]
    private float coinsGainedPerSecondLeft;

    private void Start()
    {
        levelTimer = FindObjectOfType<LevelTimer>();
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Y))
        {
            GameManager.Instance.EndGame();
        }
    }
#endif
}
