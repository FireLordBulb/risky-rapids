using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCharacter : MonoBehaviour
{
    private Player player;
    public void SetColorForPlayerOne(int color)
    {
        PlayerPrefs.SetInt("PlayerOneColor", color);
        UpdatePlayer();
    }

    public void SetColorForPlayerTwo(int color)
    {
        PlayerPrefs.SetInt("PlayerTwoColor", color);
        UpdatePlayer();
    }
    
    public void SetHairForPlayerOne(int hair)
    {
        PlayerPrefs.SetInt("PlayerOneHair", hair);
        UpdatePlayer();
    }

    public void SetHairForPlayerTwo(int hair)
    {
        PlayerPrefs.SetInt("PlayerTwoHair", hair);
        UpdatePlayer();
    }
    private void UpdatePlayer()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        player.InitializePlayerModel();
    }
}
