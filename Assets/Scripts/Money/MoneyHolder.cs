using UnityEngine;
using TMPro;

public class MoneyHolder : MonoBehaviour
{
    public int money = 0;
    public TMP_Text moneyText;

    private void Start()
    {
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money;
        }
    }
}
