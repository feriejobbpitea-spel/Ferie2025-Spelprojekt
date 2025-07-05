using UnityEngine;
using UnityEngine.Audio;
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

    [Header("Freeze Settings")]
    public float freezeDuration = 1f; // Fryser fienden i 1 sekund

    public AudioMixerGroup group;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator enemyAnimator;
    private float originalGravityScale;
    private bool isEnemyFrozen = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        spriteRenderer = (spriteRendererOverride != null) ? spriteRendererOverride : GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();

        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }
        else
        {
            Debug.LogWarning($"Ingen Rigidbody2D på {gameObject.name}");
        }

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

        FreezeEnemy(); // Fryser fienden när den tar skada

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void FreezeEnemy()
    {
        Debug.Log($"[Freeze] {gameObject.name} fryses nu i {freezeDuration} sekunder");
        isEnemyFrozen = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static; ; // Gör fienden statisk
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.speed = 0f; // Stoppa animationen
        }

        Invoke(nameof(UnfreezeEnemy), freezeDuration);
    }

    void UnfreezeEnemy()
    {
        Debug.Log($"[Freeze] {gameObject.name} återupptas");

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = originalGravityScale;
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.speed = 1f; // Återuppta animation
        }

        isEnemyFrozen = false;
    }

    void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = damageSound;
            audioSource.Play();
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
            Rigidbody2D rbCoin = coin.AddComponent<Rigidbody2D>();
            if (rbCoin != null)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
                rbCoin.linearVelocity = randomDirection * coinForce;
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
        aSource.outputAudioMixerGroup = group;
        aSource.clip = clip;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;

            Debug.Log($"{gameObject.name} ResetHealth(): maxHealth={maxHealth}, currentHealth={currentHealth}, slider.value={healthSlider.value}");
        }

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }

    public bool IsEnemyFrozen()
    {
        return isEnemyFrozen;
    }

    public bool IsAlive()
    {
        return currentHealth > 0 && gameObject.activeInHierarchy;
    }
}