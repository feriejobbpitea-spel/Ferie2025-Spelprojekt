using UnityEngine;

public class playerrespawn : MonoBehaviour
{
    public int maxLives = 3;
    private int currentLives;

    private Vector3 respawnPosition;

    void Start()
    {
        currentLives = maxLives;
        // Startposition s�tts som f�rsta respawn
        respawnPosition = transform.position;
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

        if (currentLives <= 0)
        {
            // D�r helt � h�r kan du ladda om scenen eller visa Game Over
            Debug.Log("Game Over!");
            // UnityEngine.SceneManagement.SceneManager.LoadScene(0); // valfritt
        }
        else
        {
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = respawnPosition;
        Debug.Log("Respawnar...");
        // H�r kan du �ven l�gga till animation eller reset av fiender m.m.
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPosition = newCheckpoint;
        Debug.Log("Checkpoint uppn�dd!");
    }
}