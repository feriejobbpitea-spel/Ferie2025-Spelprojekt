using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;
    private bool _collected = false;
    private Animator _animator;

    public float pickupDelay = 0f;      // Delay innan man kan plocka upp
    public float lifetime = 5f;         // Totala livstiden
    public float blinkDuration = 2f;    // Sista sekunderna den blinkar

    private float spawnTime;
    private SpriteRenderer spriteRenderer;
    private bool isBlinking = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spawnTime = Time.time;

        // Starta nedräkning för självdöd
        Invoke(nameof(SelfDestruct), lifetime);

        // Starta blinkning strax innan försvinnandet
        InvokeRepeating(nameof(Blink), lifetime - blinkDuration, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_collected && other.CompareTag("Player") && Time.time >= spawnTime + pickupDelay)
        {
            if(TryGetComponent(out Rigidbody2D rb)) 
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
            spriteRenderer.color = Color.white; // Återställ färg
        }

        if (_animator != null)
        {
            _animator.SetTrigger("Collected");
        }

        PlayerMoney.Instance.AddMoney(coinValue);

        Destroy(gameObject, 0.7f);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private void Blink()
    {
        if (spriteRenderer == null) return;

        // Växla mellan synlig och genomskinlig
        Color c = spriteRenderer.color;
        c.a = (c.a == 1f) ? 0.3f : 1f;
        spriteRenderer.color = c;
    }
}
