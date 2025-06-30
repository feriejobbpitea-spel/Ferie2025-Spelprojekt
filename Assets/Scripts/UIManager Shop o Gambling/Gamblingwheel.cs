using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gamblingwheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    public RectTransform wheel;
    public int segmentCount = 9;  // 9 segment i hjulet
    public float spinDuration = 4f;

    [Header("UI References")]
    public TMP_Text resultText;
    public Button spinButton;
    public TMP_Text moneyText;
    public TMP_Text chancesText;
    public TMP_Text costText;

    [Header("Gameplay Settings")]
    public int spinCost = 3;

    // Vinster per segment (kr)
    public int[] rewardsPerSegment = { 0, 0, 0, 0, 3, 3, 5, 8, 15 };

    // Namn per segment (valfritt)
    public string[] segmentNames = {
        "Förlust", "Förlust", "Förlust", "Förlust",
        "Win 3kr", "Win 3kr", "Win 5kr", "Win 8kr", "Jackpot 15kr"
    };

    // Sannolikheter per segment (totalt 100%)
    public float[] segmentChances = {
        11f, 11f, 11f, 11f,    // 44% förlust (0 kr)
        12.5f, 12.5f,          // 25% vinst (3 kr + 5 kr)
        10f,                   // 10% vinst (8 kr)
        5f                     // 5% jackpot (15 kr)
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
            moneyText.text = $"{PlayerMoney.Instance.money} DATACHIPS";
    }

    void ShowChancesUI()
    {
        if (chancesText == null) return;

        string chanceDisplay = "Vinstchanser:\n";

        for (int i = 0; i < segmentNames.Length; i++)
        {
            chanceDisplay += $"{segmentNames[i]}: {segmentChances[i]}%\n";
        }

        chancesText.text = chanceDisplay;
    }

    public void SpinWheel()
    {
        if (PlayerMoney.Instance.money < spinCost)
        {
            resultText.text = "Inte tillräckligt med pengar!";
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

        float random = Random.Range(0f, total);
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
        float totalAngle = 360f * 5 + targetAngle; // 5 varv + målsegment
        float elapsed = 0f;
        float startRotation = wheel.eulerAngles.z;

        while (elapsed < spinDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / spinDuration);
            float easedT = EaseOut(t);

            float newRotation = startRotation - totalAngle * easedT;
            wheel.rotation = Quaternion.Euler(0f, 0f, newRotation);

            yield return null;
        }

        // Säkerställ att hjulet stannar exakt rätt
        wheel.rotation = Quaternion.Euler(0f, 0f, startRotation - totalAngle);

        // Ge belöning
        int reward = rewardsPerSegment[segmentIndex];
        PlayerMoney.Instance.money += reward;

        resultText.text = $"Resultat: {segmentNames[segmentIndex]} (+{reward} Chip)";
        Debug.Log($"Snurrade till segment {segmentIndex}: {segmentNames[segmentIndex]} (+{reward} Chip)");

        UpdateMoneyUI();
        spinButton.interactable = true;
    }

    float EaseOut(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3);
    }
}
