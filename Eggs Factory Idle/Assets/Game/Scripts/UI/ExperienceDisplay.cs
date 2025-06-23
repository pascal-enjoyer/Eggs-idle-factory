using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private Image experienceBar;
    [SerializeField] private Text experienceText;
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

        int currentExp = PlayerEconomy.Instance.GetExperience();
        int requiredExp = PlayerEconomy.Instance.GetExperienceForNextLevel();
        int level = PlayerEconomy.Instance.GetLevel();

        experienceBar.fillAmount = (float)currentExp / requiredExp;

        experienceText.text = $"Level {level}: {currentExp}/{requiredExp}";
    }
}