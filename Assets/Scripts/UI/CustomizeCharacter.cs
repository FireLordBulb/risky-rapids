using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCharacter : MonoBehaviour
{
    private const int PlayerOne = 0, PlayerTwo = 1;
    public void SetColorForPlayerOne(RowerColorBehavior color)
    {
        SetRowerColor(PlayerOne, color);
    }
    public void SetColorForPlayerTwo(RowerColorBehavior color)
    {
        SetRowerColor(PlayerTwo, color);
    }
    public void SetMeshForPlayerOne(RowerMeshBehavior mesh)
    {
        SetRowerMesh(PlayerOne, mesh);
    }
    public void SetMeshForPlayerTwo(RowerMeshBehavior mesh)
    {
        SetRowerMesh(PlayerTwo, mesh);
    }
    private void SetRowerColor(int player, RowerColorBehavior color)
    {
        UpgradeHolder.Instance.SetRowerColor(player, color.rowerColor);
    }
    private void SetRowerMesh(int player, RowerMeshBehavior mesh)
    {
        UpgradeHolder.Instance.SetRowerMesh(player, mesh.rowerMesh);
    }
}
