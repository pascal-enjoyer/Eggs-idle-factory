using UnityEngine;

public class FloorScrollManager : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f; // ���������������� ������� (0.5f ��� ���������������)
    [SerializeField] private Transform floorsParent; // ������������ ������ ������
    [SerializeField] private Camera mainCamera; // �������� ������
    [SerializeField] private float padding = 0.5f; // ������ �� ����� ������

    private float minCameraY; // ������ ������� ������ (������ ����, ����������� Y)
    private float maxCameraY; // ������� ������� ������ (������� ����, ������������ Y)
    private bool canScroll; // ����, ����������� ������
    private Vector2 lastInputPos; // ��������� ������� �����
    private bool isDragging; // ���� ������

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        UpdateScrollBounds();
    }

    void Update()
    {
        UpdateScrollBounds();
        if (canScroll)
        {
            HandleInput();
            UpdateCameraPosition();
        }
    }

    void UpdateScrollBounds()
    {
        // ��������� ������ ������ � ������� �����������
        float screenHeight = 2f * mainCamera.orthographicSize;
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
        minCameraY = minFloorY - padding; // ������ ����
        maxCameraY = maxFloorY + padding; // ������� ����

        // ��������� ������ ���� ������
        float totalHeight = maxFloorY - minFloorY;

        // ���������, ����� �� ������
        canScroll = totalHeight + 2f * padding > screenHeight;

        // ������������ ������� ������� ������
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.y = Mathf.Clamp(cameraPos.y, minCameraY, maxCameraY);
        mainCamera.transform.position = cameraPos;

        Debug.Log($"Bounds: Floors = {floorCount}, Total Height = {totalHeight}, Screen Height = {screenHeight}, Can Scroll = {canScroll}, Min Y = {minCameraY}, Max Y = {maxCameraY}, Camera Y = {cameraPos.y}");
    }

    void HandleInput()
    {
        // ��������� ���� (���������)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastInputPos = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                float deltaY = touch.position.y - lastInputPos.y;
                float worldDeltaY = deltaY * (mainCamera.orthographicSize * 2f / Screen.height) * scrollSpeed;
                Vector3 cameraPos = mainCamera.transform.position;
                cameraPos.y -= worldDeltaY; // ����� ����� (deltaY > 0) -> ������ ����� (������� Y)
                cameraPos.y = Mathf.Clamp(cameraPos.y, minCameraY, maxCameraY);
                mainCamera.transform.position = cameraPos;
                lastInputPos = touch.position;
                Debug.Log($"Touch: deltaY = {deltaY}, worldDeltaY = {worldDeltaY}, Camera Y = {cameraPos.y}");
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        // ���� (��)
        else if (Input.GetMouseButtonDown(0))
        {
            lastInputPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentPos = Input.mousePosition;
            float deltaY = currentPos.y - lastInputPos.y;
            float worldDeltaY = deltaY * (mainCamera.orthographicSize * 2f / Screen.height) * scrollSpeed;
            Vector3 cameraPos = mainCamera.transform.position;
            cameraPos.y -= worldDeltaY; // ����� ����� (deltaY > 0) -> ������ ����� (������� Y)
            cameraPos.y = Mathf.Clamp(cameraPos.y, minCameraY, maxCameraY);
            mainCamera.transform.position = cameraPos;
            lastInputPos = currentPos;
            Debug.Log($"Mouse: deltaY = {deltaY}, worldDeltaY = {worldDeltaY}, Camera Y = {cameraPos.y}");
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