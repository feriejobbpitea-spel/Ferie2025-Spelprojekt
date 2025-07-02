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

    private float startY;
    private Vector2 targetPosition;

    private void Start()
    {
        if (ceiling == null)
        {
/*            Debug.LogError("Ceiling är inte tilldelad!");
*/            enabled = false;
            return;
        }

        startY = ceiling.position.y;
        targetPosition = new Vector2(ceiling.position.x, startY + floorY);

        if (audioSource != null && movingSound != null)
        {
            audioSource.clip = movingSound;
            audioSource.loop = true;
        }
    }

    private void Update()
    {
        if (!isActivated) return;

        float distance = Vector2.Distance(ceiling.position, targetPosition);

        if (distance > 0.01f)
        {
            StartMovingSound(); // Bara i Update, som styr nedåtgående rörelse
        }
        else
        {
            StopMovingSound();
            isActivated = false;
        }

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
        while (Vector3.Distance(ceiling.position, new Vector3(ceiling.position.x, startY + floorY, ceiling.position.z)) > 0.01f)
        {
            ceiling.position = Vector3.MoveTowards(ceiling.position, new Vector3(ceiling.position.x, startY + floorY, ceiling.position.z), speed * Time.deltaTime);
            yield return null;
        }

        StopMovingSound(); // Sluta spela när botten nåtts

        // Vänta 5 sekunder innan reset
        yield return new WaitForSeconds(5);

        // Flytta upp igen till startposition — men spela INTE ljud
        while (Vector3.Distance(ceiling.position, new Vector3(ceiling.position.x, startY, ceiling.position.z)) > 0.01f)
        {
            ceiling.position = Vector3.MoveTowards(ceiling.position, new Vector3(ceiling.position.x, startY, ceiling.position.z), speed * Time.deltaTime);
            yield return null;
        }

        isActivated = false;
    }

    private void StartMovingSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            isSoundPlaying = true;
        }
    }

    private void StopMovingSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            isSoundPlaying = false;
        }
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

