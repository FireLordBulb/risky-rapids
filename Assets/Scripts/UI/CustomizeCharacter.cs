using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCharacter : MonoBehaviour
{
    private Player player;

    private void SetColorForPlayer(int playerIndex, int color)
    {
        PlayerPrefs.SetInt(playerIndex == 0 ? "PlayerOneColor" : "PlayerTwoColor", color);
        UpdatePlayer();
    }
    public void SetColorForPlayerOne(int color)
    {
        SetColorForPlayer(0, color);
    }

    public void SetColorForPlayerTwo(int color)
    {
        SetColorForPlayer(1, color);
    }
    private void SetHairForPlayer(int playerIndex, int hair)
    {
        PlayerPrefs.SetInt(playerIndex == 0 ? "PlayerOneHair" : "PlayerTwoHair", hair);
        UpdatePlayer();
    }
    public void SetHairForPlayerOne(int hair)
    {
        SetHairForPlayer(0, hair);
    }

    public void SetHairForPlayerTwo(int hair)
    {
        SetHairForPlayer(1, hair);
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
