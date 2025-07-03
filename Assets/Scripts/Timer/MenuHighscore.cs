using UnityEngine;
using TMPro;

public class BestTimeDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text bestTimeText; // Drag dit din TextMeshProUGUI

    void Start()
    {
        if (bestTimeText == null)
        {
            Debug.LogError("BestTimeText är inte satt i BestTimeDisplay.");
            return;
        }

        float savedBestTime = LevelTimer.GetBestTime();

        Debug.Log($"[BestTimeDisplay] Hämtad tid från PlayerPrefs: {LevelTimer.FormatTime(savedBestTime)}");

        if (savedBestTime > 0f)
        {
            bestTimeText.text = $"Best Time: {LevelTimer.FormatTime(savedBestTime)}";
        }
        else
        {
            bestTimeText.text = "Best Time: --:--";
        }
    }
}