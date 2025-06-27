using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gamblingwheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    public RectTransform wheel;
    public int segmentCount = 8;
    public float spinDuration = 4f;

    [Header("UI References")]
    public TMP_Text resultText;
    public Button spinButton;
    public TMP_Text moneyText;
    public TMP_Text chancesText;
    public TMP_Text costText;

    [Header("Gameplay Settings")]
    public int spinCost = 1;

    // Bel�ningar per segment
    public int[] rewardsPerSegment = { 0, 0, 0, 0, 150, 200, 300, 500 };

    // Namn per segment
    public string[] segmentNames = {
        "F�rlust", "F�rlust", "F�rlust", "F�rlust",
        "150 kr", "200 kr", "300 kr", "Jackpot 500 kr"
    };

    // Sannolikheter per segment � totalt 100%
    public float[] segmentChances = {
        12.5f, 12.5f, 12.5f, 12.5f, 20f, 15f, 10f, 5f
    };


    void Start()
    {
        if (spinButton != null)
            spinButton.onClick.AddListener(SpinWheel);

        UpdateMoneyUI();
        ShowChancesUI();

        if (costText != null)
            costText.text = $"SPIN ({spinCost} DATACHIP)";
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"{PlayerMoney.Instance.money}";
    }

    void ShowChancesUI()
    {
        if (chancesText == null) return;

        string chanceDisplay = "Vinstchanser:\n";
        chanceDisplay += "F�rlust: 50%\n";
        chanceDisplay += "150 kr: 20%\n";
        chanceDisplay += "200 kr: 15%\n";
        chanceDisplay += "300 kr: 10%\n";
        chanceDisplay += "Jackpot 500 kr: 5%\n";

        chancesText.text = chanceDisplay;
    }

    public void SpinWheel()
    {
        if (PlayerMoney.Instance.money < spinCost)
        {
            resultText.text = "Inte tillr�ckligt med pengar!";
            return;
        }

        spinButton.interactable = false;
        PlayerMoney.Instance.money -= spinCost;
        UpdateMoneyUI();

        int chosenSegment = GetRandomSegmentBasedOnChance();
        float anglePerSegment = 360f / segmentCount;
        float targetAngle = chosenSegment * anglePerSegment;

        StartCoroutine(SpinAnimation(targetAngle, chosenSegment));
    }

    int GetRandomSegmentBasedOnChance()
    {
        float total = 0f;
        foreach (float chance in segmentChances)
            total += chance;

        float random = Random.Range(0, total);
        float cumulative = 0f;

        for (int i = 0; i < segmentChances.Length; i++)
        {
            cumulative += segmentChances[i];
            if (random < cumulative)
                return i;
        }

        return segmentChances.Length - 1; // fallback
    }

    IEnumerator SpinAnimation(float targetAngle, int segmentIndex)
    {
        float totalAngle = 360f * 5 + targetAngle;
        float elapsed = 0f;

        Quaternion startRotation = wheel.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, -totalAngle);

        while (elapsed < spinDuration)
        {
            float t = elapsed / spinDuration;
            wheel.rotation = Quaternion.Lerp(startRotation, endRotation, EaseOut(t));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        wheel.rotation = endRotation;

        int reward = rewardsPerSegment[segmentIndex];
        PlayerMoney.Instance.money += reward;

        resultText.text = $"Resultat: {segmentNames[segmentIndex]}";
        Debug.Log($"Snurrade till segment {segmentIndex}: {segmentNames[segmentIndex]}");

        UpdateMoneyUI();
        spinButton.interactable = true;
    }

    float EaseOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }
}
