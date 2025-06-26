using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class CoinTextEffect : MonoBehaviour
{
    [SerializeField] private Text text; // Текст (TMPro)
    [SerializeField] private float moveDistance = 2f; // Дистанция подъема
    [SerializeField] private float moveDuration = 1f; // Длительность анимации
    [SerializeField] private float fadeDuration = 1f; // Длительность затухания

    public void Initialize(float amount, Vector3 worldPosition, Canvas canvas)
    {
        if (text == null)
        {
            //Debug.LogWarning("CoinText: TextMeshProUGUI не назначен!");
            return;
        }

        if (canvas == null)
        {
            //Debug.LogWarning("CoinText: Canvas не передан!");
            return;
        }

        text.text = $"+{Mathf.FloorToInt(amount)}";

        // Устанавливаем родителя как Canvas
        RectTransform rect = GetComponent<RectTransform>();
        transform.SetParent(canvas.transform, false);
        transform.localScale = Vector3.one;

        // Устанавливаем как первый дочерний элемент, чтобы рендериться под другими UI
        rect.SetAsFirstSibling();
        //Debug.Log($"CoinText: Установлен как первый дочерний, индекс: {rect.GetSiblingIndex()}");

        // Конвертируем мировую позицию в локальную позицию Canvas
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            //Debug.LogWarning("CoinText: Camera.main не найдена!");
            return;
        }

        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
        Vector2 localPos;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera, out localPos))
        {
            rect.anchoredPosition = localPos;
            //Debug.Log($"CoinText: Мировая позиция: {worldPosition}, Экранная: {screenPos}, Локальная: {localPos}");
        }
        else
        {
            //Debug.LogWarning("CoinText: Не удалось преобразовать координаты!");
            rect.anchoredPosition = Vector2.zero;
        }

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * moveDistance * 100f; // Умножаем для UI-пространства
        float timer = 0f;

        Color startColor = text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            text.color = Color.Lerp(startColor, endColor, t * (fadeDuration / moveDuration));
            yield return null;
        }

        Destroy(gameObject);
    }
}