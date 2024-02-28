using UnityEngine;
using UnityEngine.UI;

public class QuitApplicationButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Application.Quit);
    }
}
