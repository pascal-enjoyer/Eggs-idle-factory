using UnityEngine;

public class FloorScrollManager : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f; // Чувствительность скролла (0.5f для универсальности)
    [SerializeField] private Transform floorsParent; // Родительский объект этажей
    [SerializeField] private Camera mainCamera; // Основная камера
    [SerializeField] private float padding = 0.5f; // Отступ от краев этажей

    private float minCameraY; // Нижняя граница камеры (нижний этаж, минимальный Y)
    private float maxCameraY; // Верхняя граница камеры (верхний этаж, максимальный Y)
    private bool canScroll; // Флаг, разрешающий скролл
    private Vector2 lastInputPos; // Последняя позиция ввода
    private bool isDragging; // Флаг слайда

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
        // Вычисляем высоту экрана в мировых координатах
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenTopY = mainCamera.transform.position.y + screenHeight / 2f;
        float screenBottomY = mainCamera.transform.position.y - screenHeight / 2f;

        // Находим реальные границы этажей
        float minFloorY = Mathf.Infinity;
        float maxFloorY = -Mathf.Infinity;
        int floorCount = floorsParent.childCount;

        if (floorCount == 0)
        {
            canScroll = false;
            return;
        }

        // Проходим по всем этажам и определяем их Y-координаты
        foreach (Transform floor in floorsParent)
        {
            float floorY = floor.position.y;
            minFloorY = Mathf.Min(minFloorY, floorY);
            maxFloorY = Mathf.Max(maxFloorY, floorY);
        }

        // Добавляем отступы
        minCameraY = minFloorY - padding; // Нижний этаж
        maxCameraY = maxFloorY + padding; // Верхний этаж

        // Вычисляем высоту всех этажей
        float totalHeight = maxFloorY - minFloorY;

        // Проверяем, нужен ли скролл
        canScroll = totalHeight + 2f * padding > screenHeight;

        // Ограничиваем текущую позицию камеры
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.y = Mathf.Clamp(cameraPos.y, minCameraY, maxCameraY);
        mainCamera.transform.position = cameraPos;

        Debug.Log($"Bounds: Floors = {floorCount}, Total Height = {totalHeight}, Screen Height = {screenHeight}, Can Scroll = {canScroll}, Min Y = {minCameraY}, Max Y = {maxCameraY}, Camera Y = {cameraPos.y}");
    }

    void HandleInput()
    {
        // Сенсорный ввод (мобильные)
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
                cameraPos.y -= worldDeltaY; // Слайд вверх (deltaY > 0) -> камера вверх (меньший Y)
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
        // Мышь (ПК)
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
            cameraPos.y -= worldDeltaY; // Слайд вверх (deltaY > 0) -> камера вверх (меньший Y)
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
        // Плавное перемещение камеры (дополнительное сглаживание)
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.y = Mathf.Lerp(cameraPos.y, cameraPos.y, Time.deltaTime * 10f);
        mainCamera.transform.position = cameraPos;
    }
}