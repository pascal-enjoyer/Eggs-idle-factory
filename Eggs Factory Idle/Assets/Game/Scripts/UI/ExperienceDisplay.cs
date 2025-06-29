using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private Image experienceBar;
    [SerializeField] private Text experienceText; 
    [SerializeField] private Color highlightColor = Color.white; 
    [SerializeField] private float blinkDuration = 2f; 
    [SerializeField] private float blinkSpeed = 2f;
    private bool isSubscribed;
    private Color originalColor;

    private void Awake()
    {
        if (experienceText != null)
        {
            originalColor = experienceText.color;
        }
    }

    private void Start()
    {
        TrySubscribe();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (PlayerEconomy.Instance != null && isSubscribed)
        {
            PlayerEconomy.Instance.ExperienceChanged -= UpdateExperienceDisplay;
            PlayerEconomy.OnLevelUp -= HandleLevelUp;
            isSubscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (!isSubscribed && PlayerEconomy.Instance != null)
        {
            PlayerEconomy.Instance.ExperienceChanged += UpdateExperienceDisplay;
            PlayerEconomy.OnLevelUp += HandleLevelUp;
            isSubscribed = true;
            UpdateExperienceDisplay();
        }
    }

    private void UpdateExperienceDisplay()
    {
        if (PlayerEconomy.Instance == null)
        {
            return;
        }

        int currentExp = PlayerEconomy.Instance.GetExperience();
        int requiredExp = PlayerEconomy.Instance.GetExperienceForNextLevel();
        int level = PlayerEconomy.Instance.GetLevel();

        experienceBar.fillAmount = (float)currentExp / requiredExp;
        experienceText.text = $"Level {level}: {currentExp}/{requiredExp}";
    }

    private void HandleLevelUp(int newLevel)
    {
        StopAllCoroutines();
        StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        float elapsedTime = 0f;
        while (elapsedTime < blinkDuration)
        {
            float t = Mathf.PingPong(elapsedTime * blinkSpeed, 1f);
            experienceText.color = Color.Lerp(originalColor, highlightColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        experienceText.color = originalColor;
    }
}