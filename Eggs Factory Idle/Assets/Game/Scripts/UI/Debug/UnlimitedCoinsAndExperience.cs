using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeleteAndReload : MonoBehaviour
{
    private Button addCoinsAndExp;

    private void Start()
    {
        addCoinsAndExp = GetComponent<Button>();
        addCoinsAndExp.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        PlayerEconomy.Instance.AddCoins(9999999);
        PlayerEconomy.Instance.AddExperience(9999999);
    }
}
