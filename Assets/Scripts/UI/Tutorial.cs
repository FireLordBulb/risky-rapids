using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject goalPanel;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button goalButton;
    private void Awake()
    {
        controlsButton.onClick.AddListener(ContinueTutorial);
        goalButton.onClick.AddListener(EndTutorial);
    }
    public void StartTutorial()
    {
        controlsPanel.SetActive(true);
    }
    private void ContinueTutorial()
    {
        controlsPanel.SetActive(false);
        goalPanel.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(null);
    }
    private void EndTutorial()
    {
        GameManager.Instance.StartCountdown(true);
        goalPanel.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(null);
    }
}
