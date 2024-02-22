using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCharacter : MonoBehaviour
{
    private const int PlayerOne = 0, PlayerTwo = 1;
    public void SetColorForPlayerOne(CharacterColorBehavior color)
    {
        SetRowerColor(PlayerOne, color);
    }
    public void SetColorForPlayerTwo(CharacterColorBehavior color)
    {
        SetRowerColor(PlayerTwo, color);
    }
    public void SetMeshForPlayerOne(CharacterMeshBehavior mesh)
    {
        SetRowerMesh(PlayerOne, mesh);
    }
    public void SetMeshForPlayerTwo(CharacterMeshBehavior mesh)
    {
        SetRowerMesh(PlayerTwo, mesh);
    }
    private void SetRowerColor(int player, CharacterColorBehavior color)
    {
        UpgradeHolder.Instance.SetRowerColor(player, color.characterColor);
    }
    private void SetRowerMesh(int player, CharacterMeshBehavior mesh)
    {
        UpgradeHolder.Instance.SetRowerMesh(player, mesh.characterMesh);
    }
}
