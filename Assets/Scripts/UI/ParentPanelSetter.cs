using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPanelSetter : MonoBehaviour
{
    [SerializeField] private ActivePanelSwitcher[] panelSwitchers;
    private void Awake()
    {
        foreach (ActivePanelSwitcher panelSwitcher in panelSwitchers)
        {
            panelSwitcher.SetParentPanel(gameObject);
        }
    }
}
