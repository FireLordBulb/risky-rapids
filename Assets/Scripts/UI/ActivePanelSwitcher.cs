using UISystem;
using UnityEngine;
using UnityEngine.UI;

public class ActivePanelSwitcher : MonoBehaviour
{
    [SerializeField] private UIObjectLinker linker;
    private GameObject parentPanel;

    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            parentPanel.SetActive(false);
            linker.GameObject.SetActive(true);
        });
    }
    public void SetParentPanel(GameObject panel)
    {
        parentPanel = panel;
    }
}
