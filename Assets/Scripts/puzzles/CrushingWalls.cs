using UnityEngine;
using System.Collections;

public class CrushingWalls : MonoBehaviour
{
    [Header("Rörelseinställningar")]
    public Transform ceiling;
    public float floorY = -3f; // Taket ska ner hit
    public float speed = 2f;
    public bool resetwalls = true; // Om väggarna ska återställas efter rörelse

    [Header("Ljudinställningar")]
    public AudioSource audioSource;
    public AudioClip movingSound;

    private bool isActivated = false;
    private bool isSoundPlaying = false;

    private float startY; // Spara startpositionens Y
    private Vector2 targetPosition;

    private void Start()
    {
        if (ceiling == null)
        {
            Debug.LogError("Ceiling är inte tilldelad!");
            enabled = false;
            return;
        }

        startY = ceiling.position.y; // Spara startpositionens Y-koordinat

        targetPosition = new Vector2(ceiling.position.x, startY - floorY);

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
        // Flytta neråt mot floorY
        ceiling.position = Vector3.MoveTowards(ceiling.position, targetPosition, speed * Time.deltaTime);
    }

    public void ActivateWalls()
    {
        if (!isActivated && resetwalls)
            StartCoroutine(MoveAndReset());
    }

    IEnumerator MoveAndReset()
    {
        isActivated = true;

        // Vänta tills taket nått botten
        while (ceiling.position.y > startY - floorY) // Liten tolerans för flyttal
        {
            yield return null;
        }

        // Vänta 5 sekunder innan reset
        yield return new WaitForSeconds(5);

        // Återställ taket till startposition
        ceiling.position = new Vector3(ceiling.position.x, startY, ceiling.position.z);
        isActivated = false;
    }

    private void OnDrawGizmos()
    {
        if (ceiling == null) return;

        Gizmos.color = Color.red;
        Vector3 targetPos = ceiling.position;
        targetPos.y = floorY;
        Gizmos.DrawWireSphere(targetPos, 0.5f);
    }
}
