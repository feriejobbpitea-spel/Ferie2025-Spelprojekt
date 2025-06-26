using UnityEngine;

public class PlayerRespawn : Singleton<PlayerRespawn>
{
    private int maxLives;
    private int currentLives;

    private Vector3 respawnPosition;

    [SerializeField] private MovingPlatform movingPlatform; // Du kan dra in manuellt, annars hittas automatiskt

    void Start()
    {
        // Hämta max liv från PlayerHealth
        maxLives = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().maxLives;
        currentLives = maxLives;

        // Försök hitta plattformen automatiskt (ny metod i Unity 2023+)
        if (movingPlatform == null)
        {
            movingPlatform = Object.FindFirstObjectByType<MovingPlatform>();
            if (movingPlatform == null)
            {
                Debug.LogWarning("Ingen MovingPlatform hittades i scenen.");
            }
        }

        // Sätt nuvarande position som start-respawnpunkt
        respawnPosition = transform.position;
    }

    public void updateMaxLives()
    {
        maxLives++;
        currentLives++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }

    void LoseLife()
    {
        currentLives--;
        Debug.Log("Liv kvar: " + currentLives);
    }

    public void Respawn()
    {
        // Flytta spelaren till senaste checkpoint
        transform.position = respawnPosition;
        Debug.Log("Respawnar...");
        currentLives = maxLives;

        // Återställ plattformen om den finns
        if (movingPlatform != null)
        {
            movingPlatform.RespawnPlatform();
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPosition = newCheckpoint;
        Debug.Log("Checkpoint uppnådd!");
    }
}