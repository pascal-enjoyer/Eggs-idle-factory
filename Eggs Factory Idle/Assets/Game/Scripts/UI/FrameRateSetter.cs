using System.Collections;
using UnityEngine;

public class FrameRateSetter : MonoBehaviour
{
    [SerializeField] private int targetFPSHigh = 90;
    [SerializeField] private int targetFPSLow = 60; 

    private static bool _isInitialized = false;

    void Awake()
    {
        if (_isInitialized)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _isInitialized = true;

        SetLandscapeOrientation();
        StartCoroutine(CheckAndSetFrameRate());
    }

    private IEnumerator CheckAndSetFrameRate()
    {
        yield return new WaitForSeconds(0.5f);

        if (DeviceSupportsHighRefreshRate())
        {
            Application.targetFrameRate = targetFPSHigh;
        }
        else
        {
            Application.targetFrameRate = targetFPSLow;
        }
    }

    private bool DeviceSupportsHighRefreshRate()
    {
        if (!Application.isMobilePlatform)
            return true;

        if (Screen.currentResolution.refreshRate >= targetFPSHigh)
            return true;

        string model = SystemInfo.deviceModel.ToLower();
        if (model.Contains("oneplus") || model.Contains("samsung galaxy s2") || model.Contains("pixel 4"))
            return true;

        return false;
    }

    private void SetLandscapeOrientation()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        if (Application.isMobilePlatform)
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
        }
    }
}
