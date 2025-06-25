using UnityEngine;

public class UICanvasRef : MonoBehaviour
{
    private static UICanvasRef instance;
    private static bool applicationIsQuitting = false;

    public static UICanvasRef Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("UICanvasRef.Instance вызван во врем€ выхода приложени€.");
                return null;
            }

            if (instance == null)
            {
                instance = FindObjectOfType<UICanvasRef>();
                if (instance == null && Application.isPlaying)
                {
                    GameObject go = new GameObject("UICanvasRef");
                    instance = go.AddComponent<UICanvasRef>();
                    Canvas canvas = go.AddComponent<Canvas>();
                }
            }
            return instance;
        }
    }

    private Canvas canvas;

    public Canvas Canvas
    {
        get
        {
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
                if (canvas == null)
                {
                    Debug.LogWarning("UICanvasRef: Canvas не найден на объекте!");
                }
            }
            return canvas;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"UICanvasRef: ќбнаружен дубликат синглтона на {gameObject.name}. ”ничтожаем этот экземпл€р.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("UICanvasRef: Canvas не найден, добавл€ем новый.");
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
        }
        Debug.Log("UICanvasRef: »нициализирован синглтон");
    }

    private void OnDestroy()
    {
        if (instance == this && !applicationIsQuitting)
        {
            Debug.LogWarning("UICanvasRef: ќсновной экземпл€р уничтожаетс€. —брасываем instance.");
            instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}