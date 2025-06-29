using System.Collections;
using UnityEngine;

public class FrameRateSetter : MonoBehaviour
{
    [SerializeField] private int targetFPSHigh = 90; // Для устройств с 90 Гц
    [SerializeField] private int targetFPSLow = 60;  // Для остальных устройств

    private static bool _isInitialized = false; // Чтобы избежать дублирования объекта

    void Awake()
    {
        // Если объект уже есть на другой сцене - уничтожаем новый экземпляр
        if (_isInitialized)
        {
            Destroy(gameObject);
            return;
        }

        // Делаем объект неуничтожаемым при загрузке сцен
        DontDestroyOnLoad(gameObject);
        _isInitialized = true;

        // Применяем настройки
        SetLandscapeOrientation();
        StartCoroutine(CheckAndSetFrameRate());
    }

    private IEnumerator CheckAndSetFrameRate()
    {
        yield return new WaitForSeconds(0.5f); // Ждём инициализации

        if (DeviceSupportsHighRefreshRate())
        {
            Application.targetFrameRate = targetFPSHigh;
            Debug.Log($"FPS: {targetFPSHigh} (High refresh rate supported)");
        }
        else
        {
            Application.targetFrameRate = targetFPSLow;
            Debug.Log($"FPS: {targetFPSLow} (Default)");
        }
    }

    private bool DeviceSupportsHighRefreshRate()
    {
        if (!Application.isMobilePlatform)
            return true; // На ПК разрешаем высокий FPS

        // Проверяем поддерживаемую частоту обновления
        if (Screen.currentResolution.refreshRate >= targetFPSHigh)
            return true;

        // Дополнительная проверка моделей устройств
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
