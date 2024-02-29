using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class ActivePanelSwitcher : MonoBehaviour
{
    private static GameObject currentActivePanel;
    
    [SerializeField] private UIObjectLinker linker;
    protected Button Button;

    protected virtual void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(Switch);
    }
    protected void Switch()
    {
        SwitchTo(linker);
    }
    public static void SwitchTo(UIObjectLinker linker)
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }
        currentActivePanel = linker.GameObject;
        currentActivePanel.SetActive(true);
    }
}
