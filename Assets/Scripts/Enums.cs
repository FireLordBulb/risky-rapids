using System;
using Unity.VisualScripting;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
    MainMenu,
    LevelComplete,
    Countdown,
    Tutorial
}

public enum UpgradeType
{
    Magnet,
    Paddle,
    Armor
}
[Serializable]
public enum RowerColor
{
    Green,
    Yellow,
    Pink,
    Purple
}
[Serializable]
public enum RowerMesh
{
    Boy,
    Girl,
    Androgynous
}