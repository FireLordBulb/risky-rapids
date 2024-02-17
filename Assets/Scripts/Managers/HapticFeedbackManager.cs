using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticFeedbackManager : MonoBehaviour
{
    public static HapticFeedbackManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Vibration.Init();
    }

    public static void CustomVibration(long ms)
    {
#if UNITY_ANDROID
        Vibration.VibrateAndroid(ms);
#endif        
    }
}
