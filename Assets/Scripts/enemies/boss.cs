using UnityEngine;
using System.Collections;
public class boss : MonoBehaviour
{
    [Header("Referenser")]
    public Transform player;              // Spelarens transform
    public Transform throwPoint;          // Punkt där kastobjekt skapas
    public Transform groundCheck;         // För att känna markkontakt

    [Header("Prefabs")]
    public GameObject throwablePrefab;    // Kastobjektet (t.ex. sten)
    public GameObject earthWavePrefab;    // Jordvågsattacken

    [Header("Attackinställningar")]
    public float jumpForce = 10f;
    public float slamDelay = 1.5f;
    public float throwForce = 10f;
    public float attackCooldown = 3f;

    [Header("Agroinställning")]
    public float agroRange = 10f;         // Bossen börjar attackera inom detta avstånd

    private Rigidbody2D rb;
    private bool isAttacking;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Vänta tills spelaren är inom agro-range
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

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        yield return new WaitForSeconds(slamDelay);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -jumpForce * 2);

        yield return new WaitUntil(() => IsGrounded());

        Instantiate(earthWavePrefab, groundCheck.position, Quaternion.identity);
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
}