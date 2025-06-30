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
    public GameObject platformPrefab;

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

    [Header("Platform Spawn Settings")]
    public int platformCount = 3;
    public float platformSpacing = 2f;
    public float platformYPosition = 3f;
    public float platformFadeDuration = 0.5f;
    public float platformLifeTime = 5f;

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
        SpawnPlatforms(); // ⬅ Spawna plattformar när bossen hoppar
        LookAtPlayer();
        OnFly?.Invoke();

        yield return new WaitForSeconds(jumpChargeTime);

        float cameraTop = Camera.main.transform.position.y + Camera.main.orthographicSize;
        float offScreenY = transform.position.y + 15;
        rb.linearVelocity = new Vector2(0, jumpForce);
        rb.gravityScale = 0f;
        SetCollisionStatus(false);

        yield return new WaitUntil(() => transform.position.y >= offScreenY);

        Vector2 targetPosition = player.position;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        var hit = Physics2D.Raycast(player.transform.position, Vector2.up, 100, groundLayer);
        rb.position = new Vector3(targetPosition.x, hit.point.y - 1, transform.position.z);

        yield return new WaitForSeconds(0.3f);

        rb.linearVelocity = new Vector2(0, -jumpForce);
        rb.gravityScale = 1f;

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
            int direction = (i % 2 == 0) ? 1 : -1;
            int distanceIndex = (i + 1) / 2;

            float xOffset = distanceIndex * earthWaveSpacing * direction;

            Vector3 spawnPosition = earthWavePoint.position + new Vector3(xOffset, -startDistanceDown, 0);
            GameObject earthWave = Instantiate(earthWavePrefab, spawnPosition, Quaternion.identity);

            earthWave.transform.DOMoveY(earthWave.transform.position.y + distanceUp, earthWaveSpeed).SetEase(Ease.InOutSine);
            StartCoroutine(DisappearEarthWave(earthWave));

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private IEnumerator DisappearEarthWave(GameObject earthWave)
    {
        yield return new WaitForSeconds(disappearDelay);

        earthWave.transform.DOMoveY(earthWave.transform.position.y - startDistanceDown, 1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(earthWave);
        });
    }

    private void SpawnPlatforms()
    {
        Debug.Log("Spawning platforms...");

        float centerX = transform.position.x;
        float y = transform.position.y - 1f; // Lite under bossens mitt

        for (int i = 0; i < platformCount; i++)
        {
            float offset = (i - (platformCount - 1) / 2f) * platformSpacing;

            // Skip spawn direkt under bossen (valfritt)
            if (Mathf.Abs(offset) < 0.1f) continue;

            Vector3 spawnPosition = new Vector3(centerX + offset, y, 0);
            GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Spawned platform at: " + spawnPosition);

            SpriteRenderer sr = platform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color startColor = sr.color;
                startColor.a = 0;
                sr.color = startColor;

                sr.DOFade(1f, platformFadeDuration);
                StartCoroutine(FadeOutAndDestroy(platform, sr));
            }
            else
            {
                Debug.LogWarning("Spawned platform has no SpriteRenderer!");
                Destroy(platform, platformLifeTime);
            }
        }
    }
    private IEnumerator FadeOutAndDestroy(GameObject obj, SpriteRenderer sr)
    {
        yield return new WaitForSeconds(platformLifeTime - platformFadeDuration);
        sr.DOFade(0f, platformFadeDuration).OnComplete(() =>
        {
            Destroy(obj);
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
