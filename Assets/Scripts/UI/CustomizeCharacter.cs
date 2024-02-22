using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCharacter : MonoBehaviour
{
    private Player player;
    public void SetColorForPlayerOne(CharacterColorBehavior color)
    {
        SetColorForPlayer(0, color);
    }
    public void SetColorForPlayerTwo(CharacterColorBehavior color)
    {
        SetColorForPlayer(1, color);
    }
    public void SetMeshForPlayerOne(CharacterMeshBehavior hair)
    {
        SetMeshForPlayer(0, hair);
    }
    public void SetMeshForPlayerTwo(CharacterMeshBehavior hair)
    {
        SetMeshForPlayer(1, hair);
    }
    private void SetColorForPlayer(int playerIndex, CharacterColorBehavior color)
    {
        PlayerPrefs.SetInt(playerIndex == 0 ? "PlayerOneColor" : "PlayerTwoColor", (int)color.characterColor);
        UpdatePlayer();
    }
    private void SetMeshForPlayer(int playerIndex, CharacterMeshBehavior hair)
    {
        PlayerPrefs.SetInt(playerIndex == 0 ? "PlayerOneHair" : "PlayerTwoHair", (int)hair.characterMesh);
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
