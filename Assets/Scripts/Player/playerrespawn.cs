using UnityEngine;

public class PlayerRespawn : Singleton<PlayerRespawn>
{
    private int maxLives;
    private int currentLives;

    private Vector3 respawnPosition;

    void Start()
    {
        maxLives = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthV2>().maxLives; // Max antal liv
        currentLives = maxLives;
        // Startposition s�tts som f�rsta respawn
       
    }

    public void updateMaxLives() {
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