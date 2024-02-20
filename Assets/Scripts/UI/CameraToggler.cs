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
            UIManager.Instance.UICameraSetActive(false);
        }
    }
    private void OnDestroy()
    {
        UIManager.Instance.UICameraSetActive(true);
    }
}
