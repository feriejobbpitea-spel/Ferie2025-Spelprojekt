using System.Collections;
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

    // Belöningar per segment
    public int[] rewardsPerSegment = { 0, 0, 0, 0, 150, 200, 300, 500 };

    // Namn per segment
    public string[] segmentNames = {
        "Förlust", "Förlust", "Förlust", "Förlust",
        "150 Datachips", "200 Datachips", "300 Datachips", "Jackpot 500 Datachips"
    };

    // Sannolikheter per segment – totalt 100%
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
        chanceDisplay += "Förlust: 50%\n";
        chanceDisplay += "150 Datachips: 20%\n";
        chanceDisplay += "200 Datachips: 15%\n";
        chanceDisplay += "300 Datachips: 10%\n";
        chanceDisplay += "Jackpot 500 Datachips: 5%\n";

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
        // Snurra 5 hela varv + slumpat segment (medurs = minska rotation i z-axeln)
        float totalAngle = 360f * 5 + targetAngle;
        float elapsed = 0f;

        // Hämta aktuell rotation i grader (0-360)
        float startRotation = wheel.eulerAngles.z;

        while (elapsed < spinDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / spinDuration);
            float easedT = EaseOut(t);

            // Beräkna ny rotation med easing, minska z för medurs snurr
            float newRotation = startRotation - totalAngle * easedT;
            wheel.rotation = Quaternion.Euler(0f, 0f, newRotation);

            yield return null;
        }

        // SäDatachipsa att hjulet stannar exakt där det ska
        wheel.rotation = Quaternion.Euler(0f, 0f, startRotation - totalAngle);

        // Ge belöning
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
