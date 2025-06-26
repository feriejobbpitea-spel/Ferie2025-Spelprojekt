using UnityEngine;
using TMPro;

public class BestTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI bestTimeText;

    void Start()
    {
        float savedTime = PlayerPrefs.GetFloat("BestTime", -1f);

        if (savedTime >= 0f)
        {
            int minutes = Mathf.FloorToInt(savedTime / 60f);
            int seconds = Mathf.FloorToInt(savedTime % 60f);
            bestTimeText.text = $"Highscore: {minutes:00}:{seconds:00}";
        }
        else
        {
            bestTimeText.text = "Highscore: --:--";
        }
    }
}
