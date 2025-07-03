using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SwingingSpike : MonoBehaviour
{
    [Header("Swing Settings")]
    public float maxAngle = 60f; // Maxvinkel i grader
    public float swingSpeed = 2f; // Hur snabbt den svingar
    public float pauseDuration = 0.5f; // Tid att pausa i ändlägena

    [Header("Audio")]
    public AudioClip swingSound;

    private AudioSource audioSource;
    private float currentAngle = 0f;
    private float targetAngle;
    private float velocity = 0f;
    private bool swingingRight = true;
    private bool isPaused = false;
    private float pauseTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        targetAngle = maxAngle;
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
                targetAngle = swingingRight ? maxAngle : -maxAngle;

                // Spela ljud när den börjar svinga
                if (swingSound && audioSource)
                {
                    audioSource.PlayOneShot(swingSound);
                }
            }
            return;
        }

        // Smooth svingrörelse
        currentAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle,
            ref velocity,
            1f / swingSpeed
        );

        transform.parent.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        // När tillräckligt nära ändläget, pausa
        if (Mathf.Abs(currentAngle - targetAngle) < 1f && Mathf.Abs(velocity) < 1f)
        {
            isPaused = true;
            pauseTimer = pauseDuration;
        }
    }
}
