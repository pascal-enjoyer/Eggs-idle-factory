using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnlimitedCoinsAndExperience : MonoBehaviour
{
    private Button deleteAllButton;

    private void Start()
    {
        deleteAllButton = GetComponent<Button>();
        deleteAllButton.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        PlayerEconomy.Instance.DeleteAll();
    }
}
