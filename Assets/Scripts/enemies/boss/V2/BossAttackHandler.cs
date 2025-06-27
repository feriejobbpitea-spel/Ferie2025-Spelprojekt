using System.Collections;
using UnityEngine;
using static BossStateController;
using DG.Tweening; 

public class BossAttackHandler : MonoBehaviour
{
    public Transform throwPoint;
    public Transform earthWavePoint;
    public Collider2D[] colliders; 

    private Transform player;
    private Rigidbody2D rb;

    [Header("Prefabs")]
    public GameObject throwablePrefab;
    public GameObject earthWavePrefab;

    [Header("Settings")]
    public float jumpChargeTime = 1f;
    public float jumpForce = 10f;
    public float slamDelay = 1.5f;
    public float throwForce = 10f;
    public float attackCooldown = 3f;
    public float hoverDuration = 2f;
    public float hoverSpeed = 2f;

    [Header("EarthWave Settings")]
    public int earthWaveCount = 5;
    public float earthWaveSpacing = 1f;
    public float earthWaveSpeed = 5f;
    public float delayBetweenWaves = 0.2f; 
    public float disappearDelay = 3f; 
    public float distanceUp = 3f;
    public float startDistanceDown = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    public delegate void AttackEvent();
    public event AttackEvent OnSlam;
    public event AttackEvent OnFly;
    public event AttackEvent OnThrow;

    private BossStateController stateController;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        stateController = GetComponent<BossStateController>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator PerformRandomAttack()
    {
        int attackType = Random.Range(0, 2);

        if (attackType == 0)
            yield return SlamAttack();
        else
            yield return ThrowAttack();

        yield return new WaitForSeconds(attackCooldown);
    }

    private void SetCollisionStatus(bool enabled) 
    {
        foreach (var item in colliders)
        {
            item.enabled = enabled;
        }
    }

    private IEnumerator SlamAttack()
    {
        LookAtPlayer();
        OnFly?.Invoke();

        yield return new WaitForSeconds(jumpChargeTime);

        // Step 1: Launch boss way above the screen
        float cameraTop = Camera.main.transform.position.y + Camera.main.orthographicSize;
        float offScreenY = 40; // 2x screen height above
        rb.linearVelocity = new Vector2(0, jumpForce); // Launch up
        rb.gravityScale = 0f;
        SetCollisionStatus(false);

        // Wait until boss is off-screen (well above the visible area)
        yield return new WaitUntil(() => transform.position.y >= offScreenY);

        // Step 2: Capture the player's position at the moment the boss disappears
        Vector2 targetPosition = player.position;

        // Step 3: Optional pause for tension
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        // Step 4: Teleport directly above the target position (off-screen again)
        var hit = Physics2D.Raycast(player.transform.position, Vector2.up, 100, groundLayer);
        rb.position = new Vector3(targetPosition.x, hit.point.y - 1, transform.position.z);

        yield return new WaitForSeconds(0.3f); // Dramatic pause before slam

        // Step 5: Slam straight down
        rb.linearVelocity = new Vector2(0, -jumpForce * 5f);

        rb.gravityScale = 1f;

        // Step 6: Wait until boss lands
        yield return new WaitUntil(() => IsGrounded());
        SetCollisionStatus(true);


        OnSlam?.Invoke();

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(SpawnEarthWaves());

        yield return new WaitForSeconds(1f);

        stateController.SetState(BossState.Vulnerable);
    }


    private IEnumerator ThrowAttack()
    {
        OnThrow?.Invoke();
        LookAtPlayer();

        yield return new WaitForSeconds(1f);

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


    private IEnumerator SpawnEarthWaves()
    {
        for (int i = 0; i < earthWaveCount; i++)
        {
            // Calculate horizontal offset: alternate left/right from center
            int direction = (i % 2 == 0) ? 1 : -1;
            // Distance multiplier: for 0 it's center, for 1 and 2 it's 1 * spacing, for 3 and 4 it's 2 * spacing, etc.
            int distanceIndex = (i + 1) / 2;

            float xOffset = distanceIndex * earthWaveSpacing * direction;

            Vector3 spawnPosition = earthWavePoint.position + new Vector3(xOffset, -startDistanceDown, 0);
            GameObject earthWave = Instantiate(earthWavePrefab, spawnPosition, Quaternion.identity);

            // Tween the earth wave upwards by distDown over earthWaveSpeed seconds
            earthWave.transform.DOMoveY(earthWave.transform.position.y + distanceUp, earthWaveSpeed).SetEase(Ease.InOutSine);

            // Start coroutine to make it disappear after delay
            StartCoroutine(DisappearEarthWave(earthWave));

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private IEnumerator DisappearEarthWave(GameObject earthWave)
    {
        yield return new WaitForSeconds(disappearDelay);

        // Move down into the ground and destroy after tween completes
        earthWave.transform.DOMoveY(earthWave.transform.position.y - startDistanceDown, 1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(earthWave);
        });
    }


    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 5f, groundLayer);
        return hit.collider != null;
    }


    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;
        scale.x = player.position.x > transform.position.x
            ? -Mathf.Abs(scale.x)
            : Mathf.Abs(scale.x);

        transform.localScale = scale;
    }
}
