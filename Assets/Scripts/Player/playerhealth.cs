using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;    // Totalt antal liv
    private int currentLives;

    void Start()
    {
        currentLives = maxLives;
        Debug.Log("Player lives: " + currentLives);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kontrollera om vi krockar med en fiende
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }

    void LoseLife()
    {
        currentLives--;
        Debug.Log("Player lost a life! Lives left: " + currentLives);

        if (currentLives <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is dead!");
        // Här kan du lägga till vad som ska hända när spelaren dör,
        // t.ex. ladda om scenen, visa Game Over, osv.
    }

}
