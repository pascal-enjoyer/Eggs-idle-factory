using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindowAnimation : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private CanvasGroup canvasGroup;
    private Vector3 initialScale;
    private bool isAnimating = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        initialScale = transform.localScale;
        // Убедимся, что окно изначально скрыто
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.zero;
    }

    public void Show()
    {
        if (isAnimating) return;
        gameObject.SetActive(true);
        StartCoroutine(ShowAnimation());
    }

    public void Hide()
    {
        if (isAnimating) return;
        StartCoroutine(HideAnimation());
    }

    private IEnumerator ShowAnimation()
    {
        isAnimating = true;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Применяем кривые анимации
            transform.localScale = initialScale * scaleCurve.Evaluate(t);
            canvasGroup.alpha = alphaCurve.Evaluate(t);

            yield return null;
        }

        // Устанавливаем финальные значения
        transform.localScale = initialScale;
        canvasGroup.alpha = 1f;
        isAnimating = false;
    }

    private IEnumerator HideAnimation()
    {
        isAnimating = true;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Применяем кривые анимации в обратном порядке
            transform.localScale = initialScale * scaleCurve.Evaluate(1f - t);
            canvasGroup.alpha = alphaCurve.Evaluate(1f - t);

            yield return null;
        }

        // Устанавливаем финальные значения и деактивируем
        transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        isAnimating = false;
    }
}