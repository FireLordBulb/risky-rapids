using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeCharacter : MonoBehaviour
{
    public void SetColorForPlayerOne(int color)
    {
        PlayerPrefs.SetInt("PlayerOneColor", color);
    }

    public void SetColorForPlayerTwo(int color)
    {
        PlayerPrefs.SetInt("PlayerTwoColor", color);
    }
    
    public void SetHairForPlayerOne(int hair)
    {
        PlayerPrefs.SetInt("PlayerOneHair", hair);
    }

    public void SetHairForPlayerTwo(int hair)
    {
        PlayerPrefs.SetInt("PlayerTwoHair", hair);
    }
}
