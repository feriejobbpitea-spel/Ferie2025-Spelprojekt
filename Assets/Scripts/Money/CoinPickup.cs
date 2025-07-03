using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public float moveSpeed = 5f; // speed at which coin moves toward player
    public float pickupDistance = 3f; // speed at which coin moves toward player

    public int coinValue = 1;
    private bool _collected = false;
    private Animator _animator;

    public float pickupDelay = 0f;
    public float lifetime = 5f;
    public float blinkDuration = 2f;

    private float spawnTime;
    private SpriteRenderer spriteRenderer;
    private bool isBlinking = false;

    private Transform player;

    [Header("Audio")]
    public AudioClip pickupSound;              // Dra in ljudfilen i Inspector
    private AudioSource audioSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        spawnTime = Time.time;

        Invoke(nameof(SelfDestruct), lifetime);
        InvokeRepeating(nameof(Blink), lifetime - blinkDuration, 0.2f);
    }

    private void Update()
    {
        if (_collected) return; // do not move if already collected

        if (player == null) return;

        float distance = Vector2.Distance(player.position, transform.position);
        if (distance < pickupDistance)
        {
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.AddForce(direction * moveSpeed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_collected && other.CompareTag("Player") && Time.time >= spawnTime + pickupDelay)
        {
            if (TryGetComponent(out Rigidbody2D rb))
            {
                Destroy(rb);
            }
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
            Collect();
        }
    }

    private void Collect()
    {
        _collected = true;

        CancelInvoke(nameof(SelfDestruct));
        CancelInvoke(nameof(Blink));
        Destroy(GetComponent<CircleCollider2D>());

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

        // F�rdr�j destruction s� att ljudet hinner spelas upp
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

