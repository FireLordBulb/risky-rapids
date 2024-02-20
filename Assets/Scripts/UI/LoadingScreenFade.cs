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
        if (timeLeft < 0)
        {
            gameObject.SetActive(false);
            isFading = false;
            return;
        }
        float opacity = timeLeft / fadeTime;
        Color newColor = new Color(baseColor.r, baseColor.g, baseColor.b, opacity);
        image.color = newColor;
    }

    public void MakeSolid()
    {
        gameObject.SetActive(true);
        image.color = baseColor;
        isFading = false;
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
