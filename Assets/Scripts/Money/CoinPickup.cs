using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;
    private bool _collected = false;
    private Animator _animator;

    public float pickupDelay = 0f;
    public float lifetime = 5f;
    public float blinkDuration = 2f;

    private float spawnTime;
    private SpriteRenderer spriteRenderer;
    private bool isBlinking = false;

    [Header("Audio")]
    public AudioClip pickupSound;              // Dra in ljudfilen i Inspector
    private AudioSource audioSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        spawnTime = Time.time;

        Invoke(nameof(SelfDestruct), lifetime);
        InvokeRepeating(nameof(Blink), lifetime - blinkDuration, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_collected && other.CompareTag("Player") && Time.time >= spawnTime + pickupDelay)
        {
            if (TryGetComponent(out Rigidbody2D rb))
            {
                Destroy(rb);
            }
            Destroy(GetComponent<CircleCollider2D>());
            Collect();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_collected && collision.collider.CompareTag("Player") && Time.time >= spawnTime + pickupDelay)
        {
            if (TryGetComponent(out Rigidbody2D rb))
            {
                Destroy(rb);
            }
            Destroy(GetComponent<CircleCollider2D>());
            Collect();
        }
    }

    private void Collect()
    {
        _collected = true;

        CancelInvoke(nameof(SelfDestruct));
        CancelInvoke(nameof(Blink));

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        if (_animator != null)
        {
            _animator.SetTrigger("Collected");
        }

        // Spela ljudet om det finns
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        PlayerMoney.Instance.AddMoney(coinValue);

        // Fördröj destruction så att ljudet hinner spelas upp
        Destroy(gameObject, 0.7f);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private void Blink()
    {
        if (spriteRenderer == null) return;

        Color c = spriteRenderer.color;
        c.a = (c.a == 1f) ? 0.3f : 1f;
        spriteRenderer.color = c;
    }
}

