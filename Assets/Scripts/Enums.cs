using System;
using Unity.VisualScripting;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
    MainMenu,
    EndGame,
    CountDown,
    Tutorial
}

public enum UpgradeType
{
    Magnet,
    Paddle,
    Armor
}
[Serializable]
public enum CharacterColor
{
    Green,
    Yellow,
    Pink,
    Purple
}
[Serializable]
public enum CharacterMesh
{
    Boy,
    Girl,
    Androgynous
}