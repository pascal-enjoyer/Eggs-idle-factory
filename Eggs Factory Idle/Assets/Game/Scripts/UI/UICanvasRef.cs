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
                Debug.LogWarning("UICanvasRef.Instance ������ �� ����� ������ ����������.");
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
                    Debug.LogWarning("UICanvasRef: Canvas �� ������ �� �������!");
                }
            }
            return canvas;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"UICanvasRef: ��������� �������� ��������� �� {gameObject.name}. ���������� ���� ���������.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("UICanvasRef: Canvas �� ������, ��������� �����.");
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
        }
        Debug.Log("UICanvasRef: ��������������� ��������");
    }

    private void OnDestroy()
    {
        if (instance == this && !applicationIsQuitting)
        {
            Debug.LogWarning("UICanvasRef: �������� ��������� ������������. ���������� instance.");
            instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}