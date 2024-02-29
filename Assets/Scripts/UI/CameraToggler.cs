using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class CameraToggler : MonoBehaviour
{
    [SerializeField] private UIObjectLinker uiCameraLinker;
    private void Awake()
    {
        if (uiCameraLinker.GameObject != null)
        {
            uiCameraLinker.GameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        if (uiCameraLinker.GameObject != null)
        {
            uiCameraLinker.GameObject.SetActive(true);
        }
    }
}
