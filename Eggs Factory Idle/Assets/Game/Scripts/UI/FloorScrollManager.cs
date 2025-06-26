using UnityEngine;

public class FloorScrollManager : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f; // Чувствительность скролла
    [SerializeField] private Transform floorsParent; // Родительский объект этажей
    [SerializeField] private Camera mainCamera; // Основная камера
    [SerializeField] private float padding = 0.5f; // Отступ от краев этажей
    [SerializeField] private float baseFov = 60f; // Базовый FOV
    [SerializeField] private float maxFov = 120f; // Максимальный FOV
    [SerializeField] private float cameraDistance = 10f; // Расстояние от камеры до плоскости этажей
    [SerializeField] private float maxFloorWidth = 10f; // Максимальная ширина этажей
    [SerializeField] private Vector2 scrollAreaMin = new Vector2(0.1f, 0.1f); // Минимальные X,Y области скролла (0-1)
    [SerializeField] private Vector2 scrollAreaMax = new Vector2(0.9f, 0.9f); // Максимальные X,Y области скролла (0-1)

    private float minCameraY; // Нижняя граница камеры (нижний этаж, минимальный Y)
    private float maxCameraY; // Верхняя граница камеры (верхний этаж, максимальный Y)
    private bool canScroll; // Флаг, разрешающий скролл
    private Vector2 lastInputPos; // Последняя позиция ввода
    private bool isDragging; // Флаг слайда
    private Vector2 lastScreenSize; // Последний размер экрана

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Устанавливаем перспективную камеру
        mainCamera.orthographic = false;
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        UpdateFovAndBounds();
    }

    void Update()
    {
        // Проверяем изменение размера экрана
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
        // Адаптируем FOV к соотношению сторон и ширине этажей
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

        // Обновляем границы
        UpdateScrollBounds();
    }

    void UpdateScrollBounds()
    {
        // Вычисляем высоту и ширину экрана в мировых координатах
        float aspectRatio = (float)Screen.width / Screen.height;
        float fovRad = mainCamera.fieldOfView * Mathf.Deg2Rad;
        float screenHeight = 2f * cameraDistance * Mathf.Tan(fovRad / 2f);
        float screenWidth = screenHeight * aspectRatio;
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
        minCameraY = minFloorY - padding; // Нижний этаж (минимальный Y)
        maxCameraY = maxFloorY + padding; // Верхний этаж (максимальный Y)

        // Вычисляем высоту всех этажей
        float totalHeight = maxFloorY - minFloorY;

        // Проверяем, нужен ли скролл
        canScroll = totalHeight + 2f * padding > screenHeight;

        // Ограничиваем текущую позицию камеры
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.y = Mathf.Clamp(cameraPos.y, minCameraY, maxCameraY);
        mainCamera.transform.position = cameraPos;

       //Debug.Log($"Bounds: Floors = {floorCount}, Total Height = {totalHeight}, Screen Height = {screenHeight}, Screen Width = {screenWidth}, FOV = {mainCamera.fieldOfView}, Can Scroll = {canScroll}, Min Y = {minCameraY}, Max Y = {maxCameraY}, Camera Y = {cameraPos.y}");
    }

    void HandleInput()
    {
        // Сенсорный ввод (мобильные)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Проверяем, попало ли касание в область скролла
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
                cameraPos.y -= worldDeltaY; // Слайд вверх (deltaY > 0) -> камера вверх (меньший Y)
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
        // Мышь (ПК)
        else if (Input.GetMouseButtonDown(0))
        {
            // Проверяем, попал ли клик в область скролла
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
            cameraPos.y -= worldDeltaY; // Слайд вверх (deltaY > 0) -> камера вверх (меньший Y)
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
        // Плавное перемещение камеры (дополнительное сглаживание)
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.y = Mathf.Lerp(cameraPos.y, cameraPos.y, Time.deltaTime * 10f);
        mainCamera.transform.position = cameraPos;
    }
}