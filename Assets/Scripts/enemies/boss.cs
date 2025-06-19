using UnityEngine;
using System.Collections;
public class boss : MonoBehaviour
{
    [Header("Referenser")]
    public Transform player;
    public Transform throwPoint;
    public Transform groundCheck;
    public Collider2D backHitbox;         // Sårbar rygg-hitbox (trigger collider)

    [Header("Prefabs")]
    public GameObject throwablePrefab;
    public GameObject earthWavePrefab;

    [Header("Attackinställningar")]
    public float jumpForce = 10f;
    public float slamDelay = 1.5f;
    public float throwForce = 10f;
    public float attackCooldown = 3f;
    public float vulnerableDuration = 2f;    // Hur länge bossen är sårbar efter landning

    [Header("Agroinställning")]
    public float agroRange = 10f;

    private Rigidbody2D rb;
    private bool isAttacking;
    private bool isVulnerable;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Se till att rygghitboxen är inaktiverad till en början
        if (backHitbox != null)
            backHitbox.enabled = false;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Kolla om spelaren är inom aggro-range
            if (Vector2.Distance(transform.position, player.position) <= agroRange)
            {
                if (!isAttacking)
                {
                    isAttacking = true;

                    int attackType = Random.Range(0, 2); // 0 = slam, 1 = kast

                    if (attackType == 0)
                        yield return StartCoroutine(SlamAttack());
                    else
                        yield return StartCoroutine(ThrowAttack());

                    yield return new WaitForSeconds(attackCooldown);
                    isAttacking = false;
                }
            }

            yield return null;
        }
    }

    private IEnumerator SlamAttack()
    {
        Debug.Log("Boss gör Slam Attack");

        // Flyg uppåt
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        yield return new WaitForSeconds(slamDelay);

        // Krasch ner
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -jumpForce * 2);

        // Vänta tills bossen är på marken
        yield return new WaitUntil(() => IsGrounded());

        // Skapa jordvågsattacken vid markkontrollpunkten
        Instantiate(earthWavePrefab, groundCheck.position, Quaternion.identity);

        // Aktivera sårbarheten på ryggen
        Debug.Log("Boss är sårbar i ryggen!");
        isVulnerable = true;
        if (backHitbox != null)
            backHitbox.enabled = true;

        // Vänta medan bossen är sårbar
        yield return new WaitForSeconds(vulnerableDuration);

        // Rotera bossen och stäng av sårbarheten
        RotateBoss();

        isVulnerable = false;
        if (backHitbox != null)
            backHitbox.enabled = false;

        Debug.Log("Boss roterade och är inte längre sårbar.");
    }

    private IEnumerator ThrowAttack()
    {
        Debug.Log("Boss gör Throw Attack");

        Vector2 direction = (player.position - throwPoint.position).normalized;

        GameObject thrownObj = Instantiate(throwablePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rbObj = thrownObj.GetComponent<Rigidbody2D>();

        if (rbObj != null)
        {
            rbObj.gravityScale = 0f;
            rbObj.linearVelocity = direction * throwForce;
        }

        yield return new WaitForSeconds(1f);
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f);
        return hit.collider != null;
    }

    private void RotateBoss()
    {
        // Roterar bossen 180 grader runt Y-axeln (flippar sprite)
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Metod för att ta skada, kallas från backHitbox trigger
    public void TakeDamage(int amount)
    {
        if (isVulnerable)
        {
            Debug.Log("Boss tar " + amount + " i skada!");
            // Lägg in HP-hantering här om du vill
        }
        else
        {
            Debug.Log("Bossen är inte sårbar just nu.");
        }
    }
}
    