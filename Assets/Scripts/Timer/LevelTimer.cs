using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance;

    private float elapsedTime = 0f;
    private bool isRunning = true;

    private TextMeshProUGUI timeDisplay;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        timeDisplay = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;

            if (timeDisplay != null)
            {
                int minutes = Mathf.FloorToInt(elapsedTime / 60f);
                int seconds = Mathf.FloorToInt(elapsedTime % 60f);
                timeDisplay.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
