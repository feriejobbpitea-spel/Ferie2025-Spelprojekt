using UnityEngine;
using TMPro;

public class PlayerMoney : Singleton<PlayerMoney>
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

    public void RemoveMoney(int amount)
    {
        money -= amount;
        if (money < 0) money = 0; // Prevent negative money
        UpdateMoneyUI();
    }   

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"{money}";
        }
    }
}
