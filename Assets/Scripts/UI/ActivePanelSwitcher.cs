using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActivePanelSwitcher : MonoBehaviour
{
    [SerializeField] private UIObjectLinker linker;
    protected Button Button;
    private GameObject parentPanel;

    protected virtual void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(SwitchPanel);
    }
    protected void SwitchPanel()
    {
        parentPanel.SetActive(false);
        linker.GameObject.SetActive(true);
    }
    public void SetParentPanel(GameObject panel)
    {
        parentPanel = panel;
    }
}
