using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToggler : MonoBehaviour
{
    private void Awake()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleUICamera(false);
        }
    }
    private void OnDestroy()
    {
        UIManager.Instance.ToggleUICamera(true);
    }
}
