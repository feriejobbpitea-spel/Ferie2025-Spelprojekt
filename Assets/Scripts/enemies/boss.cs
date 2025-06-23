using UnityEngine;
using System.Collections;
public class Boss : MonoBehaviour
{
    [Header("Referenser")]
    public Transform player;
    public Transform throwPoint;
    public Transform groundCheck;
    public Collider2D backHitbox;

    [Header("Prefabs")]
    public GameObject throwablePrefab;
    public GameObject earthWavePrefab;

    [Header("Attackinställningar")]
    public float jumpForce = 10f;
    public float slamDelay = 1.5f;
    public float throwForce = 10f;
    public float attackCooldown = 3f;
    public float vulnerableDuration = 2f;

    [Header("Agroinställning")]
    public float agroRange = 10f;

    private Rigidbody2D rb;
    private bool isAttacking;
    private bool isVulnerable;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (backHitbox != null)
            backHitbox.enabled = false;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
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
        float horizontalDirection = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(horizontalDirection * (jumpForce * 0.5f), jumpForce);

        yield return new WaitForSeconds(slamDelay);

        rb.linearVelocity = new Vector2(0f, -jumpForce * 2);

        yield return new WaitUntil(() => IsGrounded());

        Instantiate(earthWavePrefab, groundCheck.position, Quaternion.identity);

        isVulnerable = true;
        if (backHitbox != null)
            backHitbox.enabled = true;

        yield return new WaitForSeconds(vulnerableDuration);

        RotateBoss();

        isVulnerable = false;
        if (backHitbox != null)
            backHitbox.enabled = false;
    }

    private IEnumerator ThrowAttack()
    {
        Vector2 direction = (player.position - throwPoint.position).normalized;
        GameObject thrownObj = Instantiate(throwablePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rbObj = thrownObj.GetComponent<Rigidbody2D>();

        if (rbObj != null)
        {
            rbObj.gravityScale = 0f;
            rbObj.linearVelocity = direction * throwForce; // Unity 6 API
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
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(int amount)
    {
        if (isVulnerable)
        {
            Debug.Log("Boss tar " + amount + " i skada!");
        }
        else
        {
            Debug.Log("Bossen är inte sårbar just nu.");
        }
    }
}