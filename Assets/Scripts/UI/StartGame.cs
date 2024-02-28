using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : ActivePanelSwitcher
{
    protected override void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (GameManager.Instance.IsLoadingLevel)
            {
                return;
            }
            SwitchPanel();
            GameManager.Instance.StartCountdown();
        });
    }
}
