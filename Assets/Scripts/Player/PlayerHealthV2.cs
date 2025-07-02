using DG.Tweening;
using System.Collections;
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

    public AudioClip hurtSound;
    private AudioSource audioSource;

    public InventoryManager inventoryManager;

    private Vector3 lastSafePosition;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalMaterial = spriteRenderer.material;

        currentLives = maxLives;
        UpdateHearts();

        audioSource = GetComponent<AudioSource>();

        // Initiera startposition som säker
        lastSafePosition = transform.position;
    }

    void Update()
    {
        float jumpforce = movementScript.jumpForce;
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
            Vector3 hitPoint = hitT.point;
            Vector3 playerPosition = transform.position;

            
            if (currentLives >= 2)
            {
/*                Debug.Log(currentLives);
*/                TeleportToLastSafePosition();
            } 
            
            LoseLife();
  /*          Debug.Log(currentLives);*/

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
            StartCoroutine(AnimateLostHeart(hearts[currentLives]));
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
        if (maxLives < 4)
        {
            maxLives++;
        }
        if (currentLives < maxLives)
        {
            currentLives++;
        }
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < maxLives);
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

        if (inventoryManager != null)
        {
            inventoryManager.DropWeaponOnDeath();
        }

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

    private void TeleportToLastSafePosition()
    {
        transform.position = lastSafePosition;
        Debug.Log($"Teleporterad till senaste säkra plats: {lastSafePosition}");

        // 🔒 Lås rörelse i 0.2 sekunder
        StartCoroutine(LockMovementTemporarily(0.2f));
    }

    private IEnumerator AnimateLostHeart(Image heart)
    {
        // Clone the heart for animation
        GameObject animHeart = Instantiate(heart.gameObject, heart.canvas.transform);
        RectTransform animRect = animHeart.GetComponent<RectTransform>();

        // Match position and size from the original
        Vector3 worldPos = heart.rectTransform.position;
        animRect.position = worldPos;
        animRect.localScale = heart.rectTransform.lossyScale;

        Image animImage = animHeart.GetComponent<Image>();
        animImage.sprite = heart.sprite;
        animImage.SetNativeSize();
        animImage.color = Color.white;
        animHeart.transform.SetAsLastSibling(); // Ensure it's on top

        // Animate to screen center
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        animRect.DOMove(screenCenter, 0.5f).SetEase(Ease.OutQuad);
        animRect.DOScale(0.5f, 0.5f).SetEase(Ease.InOutCubic);
        animImage.DOFade(0f, 0.5f).SetEase(Ease.InExpo);

        yield return new WaitForSeconds(0.55f);
        Destroy(animHeart);
    }


    private IEnumerator LockMovementTemporarily(float duration)
    {
        if (movementScript != null)
        {
            movementScript.enabled = false;
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(duration);

        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
    }
}
