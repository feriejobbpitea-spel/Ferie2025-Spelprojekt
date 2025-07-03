using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerMoney : Singleton<PlayerMoney>
{
    public int money = 0;
    public TMP_Text moneyText;

    private int displayedMoney = 0; // Tracks current displayed value to tween from

    private void Start()
    {
        displayedMoney = money;
        UpdateMoneyUIInstant();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageDown)) 
        {
            AddMoney(9999);
        }
    }

    public void AddMoney(int amount)
    {
        int oldMoney = money;
        money += amount;
        TweenMoneyValue(oldMoney, money);
        PlayScaleAnimation();

    }

    public void RemoveMoney(int amount)
    {
        int oldMoney = money;
        money -= amount;
        if (money < 0) money = 0; // Prevent negative money
        TweenMoneyValue(oldMoney, money);
        PlayScaleAnimation();

    }

    // Instantly update the UI (no tween)
    public void UpdateMoneyUIInstant()
    {
        if (moneyText != null)
        {
            moneyText.text = $"{money}";
            Debug.Log($"Money updated instantly to: {money} at time {Time.time}");
        }
        displayedMoney = money;
    }
    private void TweenMoneyValue(int oldValue, int newValue)
    {
        DOTween.Kill(moneyText);

        DOTween.To(() => oldValue, x =>
        {
            displayedMoney = x;
            moneyText.text = displayedMoney.ToString();
        }, newValue, 0.5f).SetEase(Ease.OutCubic);
    }

    private void PlayScaleAnimation()
    {
        if (moneyText == null) return;

        moneyText.transform.DOKill(); // kill any ongoing scale tween
        moneyText.transform.localScale = Vector3.one;

        moneyText.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.4f, 10, 1);
        // Punch scale by 30% for 0.4 seconds with 10 vibrato and elasticity 1
    }
}
