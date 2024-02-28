using UnityEngine;
using UnityEngine.UI;

public class QuitApplicationWithButton : MonoBehaviour
{
    [SerializeField] private Button button;
    private void Awake()
    {
        button.onClick.AddListener(Application.Quit);
    }
}
