using UnityEngine;
using UnityEngine.UI;

public class ScrollRectInitializer : MonoBehaviour
{
    private ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        // Сбрасываем позицию прокрутки в начало
        ResetScrollPosition();
    }

    public void ResetScrollPosition()
    {
        if (scrollRect != null)
        {
            // Для вертикального скролла
            if (scrollRect.vertical)
            {
                scrollRect.verticalNormalizedPosition = 1f; // 1 - верх, 0 - низ
            }

            // Для горизонтального скролла
            if (scrollRect.horizontal)
            {
                scrollRect.horizontalNormalizedPosition = 0f; // 0 - начало, 1 - конец
            }
        }
    }
}