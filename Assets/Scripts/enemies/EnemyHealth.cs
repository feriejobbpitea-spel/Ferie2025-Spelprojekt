using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public Slider healthSlider;             // Referens till UI-slider
    public GameObject coinPrefab;           // Prefab som ska spawna när fienden dör
    public float coinForce = 5f;            // Hur hårt myntet studsar ut

    private SpriteRenderer spriteRenderer;  // För blink-effekt

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        BlinkRed();  // Blinkar rött

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void BlinkRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Invoke(nameof(ResetColor), 0.1f);
        }
    }

    void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    void Die()
    {
        if (coinPrefab != null)
        {
            // Skapa myntet vid fiendens position
            GameObject coin = Instantiate(coinPrefab, transform.position + Vector3.up, Quaternion.identity);

            // Lägg till Rigidbody2D
            Rigidbody2D rb = coin.AddComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                rb.AddForce(randomDirection * coinForce, ForceMode2D.Impulse);
            }

            // Lägg till en Collider (t.ex. CircleCollider2D) och gör den till en trigger
            CircleCollider2D collider = coin.GetComponent<CircleCollider2D>();
            collider.isTrigger = false;
        }

        Destroy(gameObject);
    }
}