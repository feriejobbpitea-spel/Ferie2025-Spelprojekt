using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance;

    [Header("Settings")]
    public TMP_Text timeDisplay; // Timer som visas under spelet
    public TMP_Text newBestText; // Text som visas om det är ny bästa tid (valfritt)

    private float elapsedTime = 0f;
    private bool isRunning = true;

    private const string BEST_TIME_KEY = "BestTime";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("LevelTimer: Instance created.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("LevelTimer: Duplicate destroyed.");
        }

        // Säkerhetsåtgärd: om ingen text är satt, leta upp automatiskt
        if (timeDisplay == null)
        {
            timeDisplay = GetComponent<TextMeshProUGUI>();
            if (timeDisplay != null)
                Debug.Log("LevelTimer: Time display auto-assigned.");
        }
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

        // Kontrollera om vi har en bättre tid än innan
        float bestTime = GetBestTime();
        Debug.Log($"LevelTimer: Current elapsed time: {FormatTime(elapsedTime)} | Best so far: {FormatTime(bestTime)}");

        if (elapsedTime < bestTime || bestTime == 0f)
        {
            PlayerPrefs.SetFloat(BEST_TIME_KEY, elapsedTime);
            PlayerPrefs.Save();

            Debug.Log($"LevelTimer: New best time saved: {FormatTime(elapsedTime)}");

            // Visa meddelande om ny bästa tid (om komponenten finns)
            if (newBestText != null)
            {
                newBestText.text = "New Best Time!";
                newBestText.gameObject.SetActive(true); // Se till att den syns
            }
        }
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    // Hämta bästa tiden från PlayerPrefs
    public static float GetBestTime()
    {
        float savedTime = PlayerPrefs.GetFloat(BEST_TIME_KEY, 0f);
        Debug.Log($"LevelTimer: Loaded best time: {FormatTime(savedTime)}");
        return savedTime;
    }

    // Returnerar formaterad tid MM:SS
    public static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{sec:00}";
    }

    // Nollställ bästa tid (kan kopplas till en knapp)
    public static void ResetBestTime()
    {
        PlayerPrefs.DeleteKey(BEST_TIME_KEY);
        PlayerPrefs.Save();
        Debug.Log("LevelTimer: Best time reset to 0.");
    }
}