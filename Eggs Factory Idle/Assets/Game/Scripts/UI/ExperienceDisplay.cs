using UnityEngine;
using UnityEngine.UI;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private Image experienceBar;
    [SerializeField] private Text experienceText; // Предполагаю Text, как в CoinDisplay
    private bool isSubscribed;

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
            isSubscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (!isSubscribed && PlayerEconomy.Instance != null)
        {
            PlayerEconomy.Instance.ExperienceChanged += UpdateExperienceDisplay;
            isSubscribed = true;
            UpdateExperienceDisplay();
        }
    }

    private void UpdateExperienceDisplay()
    {
        if (PlayerEconomy.Instance == null)
            return;

        int currentExp = PlayerEconomy.Instance.GetExperience(); // Округляем до int
        int requiredExp = PlayerEconomy.Instance.GetExperienceForNextLevel();
        int level = PlayerEconomy.Instance.GetLevel();

        experienceBar.fillAmount = (float)currentExp / requiredExp;
        experienceText.text = $"Level {level}: {currentExp}/{requiredExp}";
    }
}