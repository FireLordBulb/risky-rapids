using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class LoadNextLevel : ActivePanelSwitcher
{
    protected override void Awake()
    {
        base.Awake();
        Button.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadNextLevel();
        });
    }
}
