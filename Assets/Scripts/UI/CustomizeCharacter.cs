using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCharacter : MonoBehaviour
{
    public void SetColorForPlayerOne(CharacterColorBehavior color)
    {
        SetRowerColor(0, color);
    }
    public void SetColorForPlayerTwo(CharacterColorBehavior color)
    {
        SetRowerColor(1, color);
    }
    public void SetMeshForPlayerOne(CharacterMeshBehavior mesh)
    {
        SetRowerMesh(0, mesh);
    }
    public void SetMeshForPlayerTwo(CharacterMeshBehavior mesh)
    {
        SetRowerMesh(1, mesh);
    }
    private void SetRowerColor(int playerIndex, CharacterColorBehavior color)
    {
        UpgradeHolder.Instance.SetRowerColor(playerIndex, color.characterColor);
    }
    private void SetRowerMesh(int playerIndex, CharacterMeshBehavior mesh)
    {
        UpgradeHolder.Instance.SetRowerMesh(playerIndex, mesh.characterMesh);
    }
}
