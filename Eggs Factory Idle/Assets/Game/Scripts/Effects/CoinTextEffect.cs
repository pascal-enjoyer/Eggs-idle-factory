using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class CoinTextEffect : MonoBehaviour
{
    [SerializeField] private Text text; // ����� (TMPro)
    [SerializeField] private float moveDistance = 2f; // ��������� �������
    [SerializeField] private float moveDuration = 1f; // ������������ ��������
    [SerializeField] private float fadeDuration = 1f; // ������������ ���������

    public void Initialize(float amount, Vector3 worldPosition, Canvas canvas)
    {
        if (text == null)
        {
            //Debug.LogWarning("CoinText: TextMeshProUGUI �� ��������!");
            return;
        }

        if (canvas == null)
        {
            //Debug.LogWarning("CoinText: Canvas �� �������!");
            return;
        }

        text.text = $"+{Mathf.FloorToInt(amount)}";

        // ������������� �������� ��� Canvas
        RectTransform rect = GetComponent<RectTransform>();
        transform.SetParent(canvas.transform, false);
        transform.localScale = Vector3.one;

        // ������������� ��� ������ �������� �������, ����� ����������� ��� ������� UI
        rect.SetAsFirstSibling();
        //Debug.Log($"CoinText: ���������� ��� ������ ��������, ������: {rect.GetSiblingIndex()}");

        // ������������ ������� ������� � ��������� ������� Canvas
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            //Debug.LogWarning("CoinText: Camera.main �� �������!");
            return;
        }

        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
        Vector2 localPos;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera, out localPos))
        {
            rect.anchoredPosition = localPos;
            //Debug.Log($"CoinText: ������� �������: {worldPosition}, ��������: {screenPos}, ���������: {localPos}");
        }
        else
        {
            //Debug.LogWarning("CoinText: �� ������� ������������� ����������!");
            rect.anchoredPosition = Vector2.zero;
        }

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * moveDistance * 100f; // �������� ��� UI-������������
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