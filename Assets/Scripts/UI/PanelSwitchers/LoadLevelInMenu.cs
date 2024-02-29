using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevelInMenu : ActivePanelSwitcher
{
    [SerializeField] private int levelIndex;

    protected override void Awake()
    {
        base.Awake();
        Button.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadLevelInMenu(levelIndex);
        });
    }
}
