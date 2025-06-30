using UnityEngine;

public class CrushingWalls : MonoBehaviour
{
    [Header("Rörelseinställningar")]
    public Transform ceiling;
    public float targetY = -3f; // Y-positionen där taket ska stanna
    public float speed = 2f;

    [Header("Ljudinställningar")]
    public AudioSource audioSource;
    public AudioClip movingSound;

    private bool isActivated = false;
    private bool isSoundPlaying = false;

    private Vector2 targetPosition;

    private void Start()
    {
        if (ceiling == null)
        {
            Debug.LogError("Ceiling är inte tilldelad!");
            enabled = false;
            return;
        }

        targetPosition = new Vector2(ceiling.position.x, targetY);

        if (audioSource != null && movingSound != null)
        {
            audioSource.clip = movingSound;
            audioSource.loop = true;
        }
    }

    private void Update()
    {
        if (!isActivated) return;

        ceiling.position = Vector2.MoveTowards(ceiling.position, targetPosition, speed * Time.deltaTime);

        float distance = Vector2.Distance(ceiling.position, targetPosition);

        if (distance > 0.01f)
        {
            if (!isSoundPlaying && audioSource != null)
            {
                audioSource.Play();
                isSoundPlaying = true;
            }
        }
        else
        {
            if (isSoundPlaying && audioSource != null)
            {
                audioSource.Stop();
                isSoundPlaying = false;
                isActivated = false; // Stoppar rörelsen efter att målet nåtts
            }
        }
    }

    public void ActivateWalls()
    {
        isActivated = true;
    }

    private void OnDrawGizmos()
    {
        if (ceiling == null) return;

        Gizmos.color = Color.red;
        Vector2 previewPos = new Vector2(ceiling.position.x, targetY);
        Gizmos.DrawWireSphere(previewPos, 0.5f);
    }
}
