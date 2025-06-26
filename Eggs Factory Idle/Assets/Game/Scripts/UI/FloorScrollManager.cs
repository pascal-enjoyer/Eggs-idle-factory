using UnityEngine;

public class FloorScrollManager : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f; // ���������������� �������
    [SerializeField] private Transform floorsParent; // ������������ ������ ������
    [SerializeField] private Camera mainCamera; // �������� ������
    [SerializeField] private float padding = 0.5f; // ������ �� ����� ������
    [SerializeField] private float baseFov = 60f; // ������� FOV
    [SerializeField] private float maxFov = 120f; // ������������ FOV
    [SerializeField] private float cameraDistance = 10f; // ���������� �� ������ �� ��������� ������
    [SerializeField] private float maxFloorWidth = 10f; // ������������ ������ ������
    [SerializeField] private Vector2 scrollAreaMin = new Vector2(0.1f, 0.1f); // ����������� X,Y ������� ������� (0-1)
    [SerializeField] private Vector2 scrollAreaMax = new Vector2(0.9f, 0.9f); // ������������ X,Y ������� ������� (0-1)

    private float minCameraY; // ������ ������� ������ (������ ����, ����������� Y)
    private float maxCameraY; // ������� ������� ������ (������� ����, ������������ Y)
    private bool canScroll; // ����, ����������� ������
    private Vector2 lastInputPos; // ��������� ������� �����
    private bool isDragging; // ���� ������
    private Vector2 lastScreenSize; // ��������� ������ ������

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // ������������� ������������� ������
        mainCamera.orthographic = false;
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        UpdateFovAndBounds();
    }

    void Update()
    {
        // ��������� ��������� ������� ������
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize != lastScreenSize)
        {
            UpdateFovAndBounds();
            lastScreenSize = currentScreenSize;
        }
        else
        {
            UpdateScrollBounds();
        }

        if (canScroll)
        {
            HandleInput();
            UpdateCameraPosition();
        }
    }

    void UpdateFovAndBounds()
    {
        // ���������� FOV � ����������� ������ � ������ ������
        float aspectRatio = (float)Screen.width / Screen.height;
        float targetFov = baseFov;
        if (maxFloorWidth > 0f)
        {
            float targetWidth = maxFloorWidth + 2f * padding;
            float fovRad = 2f * Mathf.Atan(targetWidth / (2f * cameraDistance));
            targetFov = fovRad * Mathf.Rad2Deg / Mathf.Max(aspectRatio, 0.5f);
            targetFov = Mathf.Clamp(targetFov, baseFov, maxFov);
        }
        mainCamera.fieldOfView = targetFov;

        // ��������� �������
        UpdateScrollBounds();
    }

    void UpdateScrollBounds()
    {
        // ��������� ������ � ������ ������ � ������� �����������
        float aspectRatio = (float)Screen.width / Screen.height;
        float fovRad = mainCamera.fieldOfView * Mathf.Deg2Rad;
        float screenHeight = 2f * cameraDistance * Mathf.Tan(fovRad / 2f);
        float screenWidth = screenHeight * aspectRatio;
        float screenTopY = mainCamera.transform.position.y + screenHeight / 2f;
        float screenBottomY = mainCamera.transform.position.y - screenHeight / 2f;

        // ������� �������� ������� ������
        float minFloorY = Mathf.Infinity;
        float maxFloorY = -Mathf.Infinity;
        int floorCount = floorsParent.childCount;

        if (floorCount == 0)
        {
            canScroll = false;
            return;
        }

        // �������� �� ���� ������ � ���������� �� Y-����������
        foreach (Transform floor in floorsParent)
        {
            float floorY = floor.position.y;
            minFloorY = Mathf.Min(minFloorY, floorY);
            maxFloorY = Mathf.Max(maxFloorY, floorY);
        }

        // ��������� �������
        minCameraY = minFloorY - padding; // ������ ���� (����������� Y)
        maxCameraY = maxFloorY + padding; // ������� ���� (������������ Y)

        // ��������� ������ ���� ������
        float totalHeight = maxFloorY - minFloorY;

        // ���������, ����� �� ������
        canScroll = totalHeight + 2f * padding > screenHeight;

        // ������������ ������� ������� ������
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.y = Mathf.Clamp(cameraPos.y, minCameraY, maxCameraY);
        mainCamera.transform.position = cameraPos;

       //Debug.Log($"Bounds: Floors = {floorCount}, Total Height = {totalHeight}, Screen Height = {screenHeight}, Screen Width = {screenWidth}, FOV = {mainCamera.fieldOfView}, Can Scroll = {canScroll}, Min Y = {minCameraY}, Max Y = {maxCameraY}, Camera Y = {cameraPos.y}");
    }

    void HandleInput()
    {
        // ��������� ���� (���������)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // ���������, ������ �� ������� � ������� �������
                Vector2 normalizedPos = new Vector2(touch.position.x / Screen.width, touch.position.y / Screen.height);
                if (normalizedPos.x >= scrollAreaMin.x && normalizedPos.x <= scrollAreaMax.x &&
                    normalizedPos.y >= scrollAreaMin.y && normalizedPos.y <= scrollAreaMax.y)
                {
                    lastInputPos = touch.position;
                    isDragging = true;
                }
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                float deltaY = touch.position.y - lastInputPos.y;
                float worldDeltaY = deltaY * (mainCamera.fieldOfView * cameraDistance / Screen.height) * scrollSpeed;
                Vector3 cameraPos = mainCamera.transform.position;
                cameraPos.y -= worldDeltaY; // ����� ����� (deltaY > 0) -> ������ ����� (������� Y)
                cameraPos.y = Mathf.Clamp(cameraPos.y, minCameraY, maxCameraY);
                mainCamera.transform.position = cameraPos;
                lastInputPos = touch.position;
                //Debug.Log($"Touch: deltaY = {deltaY}, worldDeltaY = {worldDeltaY}, Camera Y = {cameraPos.y}");
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        // ���� (��)
        else if (Input.GetMouseButtonDown(0))
        {
            // ���������, ����� �� ���� � ������� �������
            Vector2 normalizedPos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            if (normalizedPos.x >= scrollAreaMin.x && normalizedPos.x <= scrollAreaMax.x &&
                normalizedPos.y >= scrollAreaMin.y && normalizedPos.y <= scrollAreaMax.y)
            {
                lastInputPos = Input.mousePosition;
                isDragging = true;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentPos = Input.mousePosition;
            float deltaY = currentPos.y - lastInputPos.y;
            float worldDeltaY = deltaY * (mainCamera.fieldOfView * cameraDistance / Screen.height) * scrollSpeed;
            Vector3 cameraPos = mainCamera.transform.position;
            cameraPos.y -= worldDeltaY; // ����� ����� (deltaY > 0) -> ������ ����� (������� Y)
            cameraPos.y = Mathf.Clamp(cameraPos.y, minCameraY, maxCameraY);
            mainCamera.transform.position = cameraPos;
            lastInputPos = currentPos;
            //Debug.Log($"Mouse: deltaY = {deltaY}, worldDeltaY = {worldDeltaY}, Camera Y = {cameraPos.y}");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void UpdateCameraPosition()
    {
        // ������� ����������� ������ (�������������� �����������)
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.y = Mathf.Lerp(cameraPos.y, cameraPos.y, Time.deltaTime * 10f);
        mainCamera.transform.position = cameraPos;
    }
}