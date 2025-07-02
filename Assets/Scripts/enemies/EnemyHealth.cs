using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Hälsa")]
    public int maxHealth = 3;
    private int currentHealth;
    public Slider healthSlider;

    [Header("Belöning")]
    public GameObject coinPrefab;
    public float coinForce = 5f;

    [Header("Overrides")]
    public GameObject toRemove;
    public SpriteRenderer spriteRendererOverride;

    [Header("Ljud")]
    public AudioClip damageSound;
    public AudioSource audioSource;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        spriteRenderer = (spriteRendererOverride != null) ? spriteRendererOverride : GetComponent<SpriteRenderer>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log($"{gameObject.name} TakeDamage(): took {amount} damage, currentHealth={currentHealth}", gameObject);

        PlayDamageSound();
        BlinkRed();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
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
        PlaySoundAtPosition(damageSound, transform.position);

        if (coinPrefab != null)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = coin.AddComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                rb.AddForce(randomDirection * coinForce, ForceMode2D.Impulse);
            }
        }

        Destroy((toRemove != null) ? toRemove : gameObject);
    }

    void PlaySoundAtPosition(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }

    // Ny metod för att återställa hälsa och uppdatera UI
    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;  // Viktigt att maxValue är rätt
            healthSlider.value = currentHealth;

            Debug.Log($"{gameObject.name} ResetHealth(): maxHealth={maxHealth}, currentHealth={currentHealth}, slider.value={healthSlider.value}");
        }

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }

    // Ny metod för att kolla om fienden lever
    public bool IsAlive()
    {
        return currentHealth > 0 && gameObject.activeInHierarchy;
    }
}
