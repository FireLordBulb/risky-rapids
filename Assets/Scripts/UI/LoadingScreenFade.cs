using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenFade : MonoBehaviour
{
    [SerializeField] private float fadeTime;
    private Image image;
    private Color baseColor;
    private float timeLeft;
    private bool isFading;
    private void Awake()
    {
        image = GetComponent<Image>();
        baseColor = image.color;
    }
    private void Update()
    {
        if (!isFading)
        {
            return;
        }
        timeLeft -= Time.deltaTime;
        float opacity = timeLeft / fadeTime;
        if (timeLeft < 0)
        {
            gameObject.SetActive(false);
            isFading = false;
            opacity = 1;
        }
        Color newColor = new Color(baseColor.r, baseColor.g, baseColor.b, opacity);
        image.color = newColor;
    }
    public void FadeOut()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        timeLeft = fadeTime;
        isFading = true;
    }
}
