using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SwingingSpike : MonoBehaviour
{
    [Header("Swing Settings")]
    public float maxAngle = 60f;
    public float swingDuration = 2f; // Hur lång tid en sväng tar (fram och tillbaka)
    public float pauseDuration = 0.5f;

    [Header("Audio")]
    public AudioClip swingSound;
    public float activationRadius = 10f;

    private AudioSource audioSource;
    private Transform player;

    private bool swingingRight = true;
    private bool isPaused = false;
    private float pauseTimer = 0f;

    private float swingTimer = 0f; // Tid i svängen
    private float startAngle;
    private float endAngle;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Ingen spelare hittades med taggen 'Player'.");
        }

        // Initiera första svängen
        swingingRight = true;
        startAngle = -maxAngle;
        endAngle = maxAngle;
        swingTimer = 0f;
    }

    void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                swingingRight = !swingingRight;

                // Växla riktning och vinklar
                if (swingingRight)
                {
                    startAngle = -maxAngle;
                    endAngle = maxAngle;
                }
                else
                {
                    startAngle = maxAngle;
                    endAngle = -maxAngle;
                }

                swingTimer = 0f;

                // Spela ljud om spelaren är nära
                if (swingSound && audioSource && player != null)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                    if (distanceToPlayer <= activationRadius)
                    {
                        audioSource.PlayOneShot(swingSound);
                    }
                }
            }
            return;
        }

        // Uppdatera swingTimer
        swingTimer += Time.deltaTime;
        float t = swingTimer / swingDuration;
        t = Mathf.Clamp01(t);

        // Ease in/out med sinus (mjukt start och slut)
        float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

        // Interpolera vinkeln
        float currentAngle = Mathf.Lerp(startAngle, endAngle, easedT);
        transform.parent.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        if (t >= 1f)
        {
            isPaused = true;
            pauseTimer = pauseDuration;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}


