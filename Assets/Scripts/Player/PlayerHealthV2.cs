using DG.Tweening;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthV2 : Singleton<PlayerHealthV2>
{
    public int maxLives = 3;
    public int currentLives;

    public Image[] hearts;
    public Sprite emptyHeart;
    public Image death;
    public Image pause;
    public Image gameOver;

    private bool isInvincible = false;
    public float invincibilityDuration = 0.8f;
    private float invincibilityTimer;

    private SpriteRenderer spriteRenderer;
    private bool toggleWhite = false;
    private float blinkTimer = 0f;
    public float blinkInterval = 0.1f;

    public Material whiteFlashMaterial;
    private Material originalMaterial;
    private Rigidbody2D rb;

    public Movement movementScript;
    private bool wasGroundedLastFrame = true;
    private float lastYVelocity;
    public float fallLimit = -10f;

    public LayerMask trapLayer;
    public Vector2 boxCastSizeT = new Vector2(1f, 1.5f);
    public float boxCastDistanceT = 0.1f;

    public LayerMask enemyLayer;
    public Vector2 boxCastSizeE = new Vector2(1f, 1.5f);
    public float boxCastDistanceE = 0.1f;

    public LayerMask groundLayer;

    public AudioClip hurtSound;
    private AudioSource audioSource;

    public InventoryManager inventoryManager;

    private Vector3 lastSafePosition;
    private Transform currentMovingPlatform = null;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalMaterial = spriteRenderer.material;

        currentLives = maxLives;
        UpdateHearts();

        audioSource = GetComponent<AudioSource>();

        lastSafePosition = transform.position;
    }

    void Update()
    {
        
        if (currentMovingPlatform != null)
        {
            if (!currentMovingPlatform.GetComponent<SpriteRenderer>().enabled) { currentMovingPlatform = null; }


            if (currentMovingPlatform.gameObject == null)
            {
                currentMovingPlatform = null;
            }
            else if (currentMovingPlatform.GetComponent<SpriteRenderer>() != null)
            {
                Debug.Log($"Current Moving Platform: {currentMovingPlatform.gameObject.activeInHierarchy}");
                lastSafePosition = currentMovingPlatform.position;
                lastSafePosition.y += 0.6f;
            }

        }
       
        Vector2 origin = rb.position;
        Vector2 direction = Vector2.right * Mathf.Sign(transform.localScale.x);

        RaycastHit2D hitT = Physics2D.BoxCast(origin, boxCastSizeT, 0f, direction, boxCastDistanceT, trapLayer);
        RaycastHit2D hitE = Physics2D.BoxCast(origin, boxCastSizeE, 0f, direction, boxCastDistanceE, enemyLayer);

        if (movementScript.isGrounded && hitT.collider == null)
        {
            lastSafePosition = transform.position;
        }

        if (hitT.collider != null)
        {
            if (currentLives > 1 && !isInvincible)
            {
                SafeTeleportToLastSafePosition();
                
            }
            LoseLife();

        }

        if (hitE.collider != null)
        {
            LoseLife();
        }

        if (Input.GetKeyDown(KeyCode.J)) AddLife();

        bool isGrounded = movementScript.isGrounded;

        if (isGrounded && !wasGroundedLastFrame)
        {
            if (lastYVelocity < fallLimit)
            {
                LoseLife();
            }
        }

        wasGroundedLastFrame = isGrounded;
        lastYVelocity = rb.linearVelocity.y;

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;

            if (blinkTimer <= 0f)
            {
                toggleWhite = !toggleWhite;
                blinkTimer = blinkInterval;
                spriteRenderer.color = toggleWhite ? Color.white : Color.clear;
            }

            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                spriteRenderer.color = Color.white;
            }
        }

        
        if (Input.GetKeyDown(KeyCode.H)) LoseLife();
    }

    public void LoseLife()
    {
        if (isInvincible) return;

        PlaySound(hurtSound);
        CameraFollow.Instance?.TriggerShake(0.15f, 0.2f);

        currentLives--;
        if (currentLives < hearts.Length && currentLives >= 0)
        {
            StartCoroutine(AnimateHeartWrapperLoss());
        }
        UpdateHearts();

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        if (currentLives <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashWhite());
        }
    }

    public void AddLife()
    {
        if (maxLives < 4) maxLives++;
        if (currentLives < maxLives) currentLives++;
        UpdateHearts();
        StartCoroutine(AnimateHeartWrapperGain());
    }
    public void AddSingleLife()
    { if (currentLives < maxLives) currentLives++;
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < maxLives);
            hearts[i].rectTransform.localScale = Vector3.one;
        }

        for (int i = 0; i < maxLives; i++)
        {
            if (i < currentLives)
            {
                var heartUI = BiomeHandler.Instance.CurrentBiome.HeartUI;
                hearts[i].sprite = heartUI;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    void Die()
    {
        PlaySound(hurtSound);

        if (maxLives == 1)
            gameOver.gameObject.SetActive(true);
        else
            death.gameObject.SetActive(true);

        inventoryManager?.DropWeaponOnDeath();
        Time.timeScale = 0;
    }

    public void SuperRespawn()
    {
        Time.timeScale = 1;
        PlayerRespawn.Instance.RespawnAtSuperCheckpoint();
        maxLives = 3;
        currentLives = maxLives;
        UpdateHearts();
        gameOver.gameObject.SetActive(false);
        isInvincible = true;
        invincibilityTimer = invincibilityDuration * 2;
    }

    public void Respawn()
    {
        Time.timeScale = 1;
        PlayerRespawn.Instance.Respawn();
        maxLives--;
        currentLives = maxLives;
        UpdateHearts();
        death.gameObject.SetActive(false);
        isInvincible = true;
        invincibilityTimer = invincibilityDuration * 2;
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.material = whiteFlashMaterial;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material = originalMaterial;
    }

    public void tryAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameOver.gameObject.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private bool IsPositionFree(Vector2 position)
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("❌ Spelar-collider saknas!");
            return false;
        }

        Vector2 size = playerCollider.bounds.size * 0.85f;
        Collider2D[] hits = Physics2D.OverlapBoxAll(position, size, 0f, groundLayer);

        foreach (Collider2D hit in hits)
        {
            if (!hit.isTrigger)
            {
                Debug.LogWarning($"❌ Blockerad av {hit.name}");
                DebugDrawBox(position, size, Color.red);
                return false;
            }
        }

        DebugDrawBox(position, size, Color.green);
        return true;
    }

    private void DebugDrawBox(Vector2 center, Vector2 size, Color color)
    {
        Vector3 pos = new Vector3(center.x, center.y, 0f);
        Vector3 halfSize = new Vector3(size.x, size.y, 0f) / 2f;

        Debug.DrawLine(pos - halfSize, pos + new Vector3(-halfSize.x, halfSize.y, 0), color, 0.5f);
        Debug.DrawLine(pos + new Vector3(-halfSize.x, halfSize.y, 0), pos + halfSize, color, 0.5f);
        Debug.DrawLine(pos + halfSize, pos + new Vector3(halfSize.x, -halfSize.y, 0), color, 0.5f);
        Debug.DrawLine(pos + new Vector3(halfSize.x, -halfSize.y, 0), pos - halfSize, color, 0.5f);
    }

    private bool SafeTeleportToLastSafePosition()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("❌ Spelar-collider saknas!");
            return false;
        }

        Vector2 originalSize = playerCollider.bounds.size;
        Vector2 testSize = originalSize * 0.85f;
        float verticalOffset = testSize.y / 2f + 0.05f;

        Vector2 basePosition = new Vector2(lastSafePosition.x+((transform.position.x-lastSafePosition.x)/math.abs((transform.position.x-lastSafePosition.x))*-0.5f), lastSafePosition.y + verticalOffset);

        
        if (IsPositionFree(basePosition))
        {
            Teleport(basePosition);
            return true;
        }

        int maxSteps = 40;
        float step = 0.1f;

        for (int i = 1; i <= maxSteps; i++)
        {
            Vector2 testPos = basePosition + Vector2.up * (i * step);
            if (IsPositionFree(testPos))
            {
                Teleport(testPos);
                return true;
            }
        }

    

        Debug.LogError("❌ Kunde inte hitta säker teleport-position.");
        return false;
    }

    private void Teleport(Vector2 newPosition)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = newPosition;
        StartCoroutine(LockMovementTemporarily(0.4f));
    }
    private IEnumerator AnimateHeartWrapperLoss()
    {
        if (currentLives >= 0 && currentLives < hearts.Length)
        {
            Image heart = hearts[currentLives];
            var rt = heart.rectTransform;

            // Reset scale first
            rt.localScale = Vector3.one;

            Sequence seq = DOTween.Sequence();
            seq.Append(rt.DOPunchScale(new Vector3(-0.3f, 0.3f, 0), 0.3f, 10, 1));
            seq.Join(heart.DOColor(Color.red, 0.15f).SetLoops(2, LoopType.Yoyo));

            yield return seq.WaitForCompletion();
            rt.localScale = Vector3.one; // Ensure reset
        }
    }


    private IEnumerator AnimateHeartWrapperGain()
    {
        if (currentLives - 1 >= 0 && currentLives - 1 < hearts.Length)
        {
            Image heart = hearts[currentLives - 1];
            var rt = heart.rectTransform;

            rt.localScale = Vector3.one * 0.5f;

            yield return rt
                .DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutBack)
                .WaitForCompletion();

            rt.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 1);
        }
    }


    private IEnumerator LockMovementTemporarily(float duration)
    {
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        yield return new WaitForSeconds(duration);

        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MovingPlatform"))
        {
            if (currentMovingPlatform == null)
            {
                currentMovingPlatform = collision.transform;
            }
        }
    }
}
