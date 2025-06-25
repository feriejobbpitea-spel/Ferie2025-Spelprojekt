using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public Slider healthSlider;             // Referens till UI-slider
    public GameObject coinPrefab;           // Prefab som ska spawna n�r fienden d�r
    public float coinForce = 5f;            // Hur h�rt myntet studsar ut

    [Header("Overrides")]
    public GameObject toRemove;
    public SpriteRenderer spriteRendererOverride;

    private SpriteRenderer spriteRenderer;  // F�r blink-effekt

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        spriteRenderer = (spriteRendererOverride != null) ? spriteRendererOverride : GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        BlinkRed();  // Blinkar r�tt

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

    public virtual void Die()
    {
        if (coinPrefab != null)
        {
            // Skapa myntet vid fiendens position
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            // L�gg till kraft s� den studsar ut
            Rigidbody2D rb = coin.AddComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                rb.AddForce(randomDirection * coinForce, ForceMode2D.Impulse);
            }
        }

        Destroy((toRemove != null) ? toRemove :  gameObject);
    }
}
