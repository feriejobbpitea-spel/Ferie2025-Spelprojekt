using UnityEngine;

public class VerticalMovingPlatform : MonoBehaviour
{
    [Header("Rörelseinställningar")]
    public float speed = 2f;
    public float verticalDistance = 3f;
    public float horizontalDistance = 10f;

    [Header("Aktivering")]
    public float activationRadius = 5f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 previousPosition;
    private bool movingUp = true;
    private bool isActive = false;
    private void ClearProjectiles()
    {
        Vector2 boxCenter = rb.position;
        Vector2 boxSize = GetComponent<Collider2D>().bounds.size;

        // Hitta alla colliders som överlappar med plattformen
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);

        foreach (var hit in hits)
        {
            if (hit != null && hit.gameObject.layer == LayerMask.NameToLayer("EnemyProjectiles"))
            {
                Destroy(hit.gameObject);
            }
        }
    }

    private Transform playerTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;
        previousPosition = rb.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Ingen spelare hittades med taggen 'Player'");
        }
    }

    void OnEnable()
    {
        PlayerRespawn.OnPlayerRespawn += RespawnPlatform;
    }

    void OnDisable()
    {
        PlayerRespawn.OnPlayerRespawn -= RespawnPlatform;
    }

    void FixedUpdate()
    {
        if (!isActive && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(rb.position, playerTransform.position);
            if (distanceToPlayer <= activationRadius)
            {
                isActive = true;
                Debug.Log("Plattform aktiverad av spelare.");
            }
            else
            {
                return;
            }
        }

        if (!isActive) return;

        // Rörelse
        Vector2 horizontalMovement = Vector2.right * speed * Time.fixedDeltaTime;
        float direction = movingUp ? 1f : -1f;
        Vector2 verticalMovement = Vector2.up * direction * speed * Time.fixedDeltaTime;
        Vector2 newPosition = rb.position + horizontalMovement + verticalMovement;

        // Vertikal riktning
        if (movingUp && newPosition.y >= startPos.y + verticalDistance)
        {
            movingUp = false;
        }
        else if (!movingUp && newPosition.y <= startPos.y)
        {
            movingUp = true;
        }

        // Horisontell gräns – avaktivera plattform
        if (newPosition.x >= startPos.x + horizontalDistance)
        {
            gameObject.SetActive(false);
            return;
        }

        rb.MovePosition(newPosition);
        previousPosition = rb.position;

        // 🔍 Rensa projektiler med BoxCast
        ClearProjectiles();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyProjectiles"))
        {
            Destroy(collision.gameObject);
        }
    }

    public void RespawnPlatform()
    {
        Debug.Log("Vertikal plattform respawnar");
        rb.position = startPos;
        previousPosition = startPos;
        movingUp = true;
        isActive = false;
        gameObject.SetActive(true);
    }
}