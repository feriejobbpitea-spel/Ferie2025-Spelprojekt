using UnityEngine;
using System.Collections;
using System;
public class Boss : MonoBehaviour
{
    [Header("Referenser")]
    private Transform player;
    public Transform throwPoint;
    public Transform earthWavePoint;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject bossHealthBar;

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

    public event Action OnSlam;
    public event Action OnFly;
    public event Action OnThrow;

    private bool _hasSeenPlayer = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the tag 'Player'.");
        }
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, player.position) <= agroRange)
            {
                bossHealthBar.SetActive(true);
                _hasSeenPlayer = true;
            }
            else 
            {
                bossHealthBar.SetActive(false);
            }
            if (_hasSeenPlayer)
            {
                if (!isAttacking)
                {
                    isAttacking = true;

                    int attackType = UnityEngine.Random.Range(0, 2); // 0 = slam, 1 = kast
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
        LookAtPlayer();
        OnFly?.Invoke(); // <-- Flying up
        yield return new WaitForSeconds(1f);
        LookAtPlayer();
        float horizontalDirection = Mathf.Sign(player.position.x - transform.position.x);


        rb.linearVelocity = new Vector2(horizontalDirection * (jumpForce * 0.5f), jumpForce);

        yield return new WaitForSeconds(slamDelay);

        rb.linearVelocity = new Vector2(0f, -jumpForce * 2); // Slam down

        yield return new WaitUntil(() => IsGrounded());


        OnSlam?.Invoke(); // <-- Slam hits ground
        Instantiate(earthWavePrefab, earthWavePoint.position, Quaternion.identity);

        yield return new WaitForSeconds(1);
        isVulnerable = true;
        LookAwayFromPlayer();
        yield return new WaitForSeconds(vulnerableDuration);

        LookAtPlayer();

        isVulnerable = false;
    }


    private IEnumerator ThrowAttack()
    {
        OnThrow?.Invoke(); // <-- Event for throwing
        LookAtPlayer();

        yield return new WaitForSeconds(1f);

        LookAtPlayer();
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
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;

        // If player is to the right, ensure scale.x is positive (facing right)
        // If player is to the left, scale.x should be negative (facing left)
        if (player.position.x > transform.position.x)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    private void LookAwayFromPlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;

        if (player.position.x > transform.position.x)
            scale.x = Mathf.Abs(scale.x);   // Face left (away)
        else
            scale.x = -Mathf.Abs(scale.x);    // Face right (away)

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