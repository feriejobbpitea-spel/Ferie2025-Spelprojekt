using UnityEngine;

public class PlayerRespawn : Singleton<PlayerRespawn>
{
    public int maxLives = 3;
    private int currentLives;

    private Vector3 respawnPosition;

    void Start()
    {
        currentLives = maxLives;
        // Startposition s�tts som f�rsta respawn
       
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
        transform.position = respawnPosition;
        Debug.Log("Respawnar...");
        // H�r kan du �ven l�gga till animation eller reset av fiender m.m.
        currentLives = maxLives; // �terst�ll liv vid respawn
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPosition = newCheckpoint;
        Debug.Log("Checkpoint uppn�dd!");
    }
}