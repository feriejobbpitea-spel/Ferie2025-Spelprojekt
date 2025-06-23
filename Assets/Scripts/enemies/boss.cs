using UnityEngine;
using System.Collections;
public class Boss : MonoBehaviour
{
    [Header("Referenser")]
    public Transform player;
    public Transform throwPoint;
    public Transform groundCheck;
    public Collider2D backHitbox;         // S�rbar rygg-hitbox (trigger collider)

    [Header("Prefabs")]
    public GameObject throwablePrefab;
    public GameObject earthWavePrefab;

    [Header("Attackinst�llningar")]
    public float jumpForce = 10f;
    public float slamDelay = 1.5f;
    public float throwForce = 10f;
    public float attackCooldown = 3f;
    public float vulnerableDuration = 2f;    // Hur l�nge bossen �r s�rbar efter landning

    [Header("Agroinst�llning")]
    public float agroRange = 10f;

    private Rigidbody2D rb;
    private bool isAttacking;
    private bool isVulnerable;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Se till att rygghitboxen �r inaktiverad till en b�rjan
        if (backHitbox != null)
            backHitbox.enabled = false;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Kolla om spelaren �r inom aggro-range
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
        Debug.Log("Boss g�r Slam Attack");

        // R�kna ut riktningen mot spelaren i X-led
        float horizontalDirection = Mathf.Sign(player.position.x - transform.position.x);

        // S�tt bossens velocity f�r att hoppa mot spelaren
        rb.linearVelocity = new Vector2(horizontalDirection * (jumpForce * 0.5f), jumpForce);
        yield return new WaitForSeconds(slamDelay);

        // Krasch ner (rakt ner)
        rb.linearVelocity = new Vector2(0f, -jumpForce * 2);

        // V�nta tills bossen �r p� marken
        yield return new WaitUntil(() => IsGrounded());

        // Skapa jordv�gsattacken vid markkontrollpunkten
        Instantiate(earthWavePrefab, groundCheck.position, Quaternion.identity);

        // Aktivera s�rbarheten p� ryggen
        Debug.Log("Boss �r s�rbar i ryggen!");
        isVulnerable = true;
        if (backHitbox != null)
            backHitbox.enabled = true;

        // V�nta medan bossen �r s�rbar
        yield return new WaitForSeconds(vulnerableDuration);

        // Rotera bossen och st�ng av s�rbarheten
        RotateBoss();

        isVulnerable = false;
        if (backHitbox != null)
            backHitbox.enabled = false;

        Debug.Log("Boss roterade och �r inte l�ngre s�rbar.");
    }

    private IEnumerator ThrowAttack()
    {
        Debug.Log("Boss g�r Throw Attack");

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

    // Metod f�r att ta skada, kallas fr�n backHitbox trigger
    public void TakeDamage(int amount)
    {
        if (isVulnerable)
        {
            Debug.Log("Boss tar " + amount + " i skada!");
            // L�gg in HP-hantering h�r om du vill
        }
        else
        {
            Debug.Log("Bossen �r inte s�rbar just nu.");
        }
    }
}
