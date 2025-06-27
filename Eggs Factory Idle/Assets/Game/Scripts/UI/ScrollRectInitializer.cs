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
        // ���������� ������� ��������� � ������
        ResetScrollPosition();
    }

    public void ResetScrollPosition()
    {
        if (scrollRect != null)
        {
            // ��� ������������� �������
            if (scrollRect.vertical)
            {
                scrollRect.verticalNormalizedPosition = 1f; // 1 - ����, 0 - ���
            }

            // ��� ��������������� �������
            if (scrollRect.horizontal)
            {
                scrollRect.horizontalNormalizedPosition = 0f; // 0 - ������, 1 - �����
            }
        }
    }
}