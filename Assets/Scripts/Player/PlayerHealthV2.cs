using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthV2 : Singleton<PlayerHealthV2>
{
    public int maxLives = 3;
    public int currentLives;

    public Image[] hearts;         // Dra in tre Image-objekt från Canvas
    public Sprite emptyHeart;      // Grått/tomt hjärta
    public Image death;
    public Image pause;
    public Image gameOver;
    private bool isInvincible = false;
    public float invincibilityDuration = 0.8f; // hur länge man är odödlig
    private float invincibilityTimer;

    private SpriteRenderer spriteRenderer;
    private bool toggleWhite = false;
    private float blinkTimer = 0f;
    public float blinkInterval = 0.1f; // hur snabbt det blinkar

    public Material whiteFlashMaterial;  // Det vita materialet
    private Material originalMaterial;   // För att spara spelarens normala material
    private Rigidbody2D rb;

    public Movement movementScript;
    private bool wasGroundedLastFrame = true;
    private float lastYVelocity;
    public float fallLimit = -10f; // Gräns för fallskada, justera efter behov

    // Nytt för BoxCast
    public LayerMask enemyLayer;                // Fiendelayer
    public Vector2 boxCastSize = new Vector2(1f, 1.5f);  // Storlek på boxen
    public float boxCastDistance = 0.1f;        // Hur långt framför spelaren boxen kastas

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalMaterial = spriteRenderer.material;
        currentLives = maxLives;
        UpdateHearts();
        Debug.Log("Player lives: " + currentLives);
        Debug.Log("SpriteRenderer: " + spriteRenderer);
    }

    void Update()
    {
        // BoxCast för att kolla fiender framför spelaren
        Vector2 origin = rb.position;
        Vector2 direction = Vector2.right * Mathf.Sign(transform.localScale.x);
        RaycastHit2D hit = Physics2D.BoxCast(origin, boxCastSize, 0f, direction, boxCastDistance, enemyLayer);
        if (hit.collider != null)
        {
            Debug.Log("BoxCast hit enemy: " + hit.collider.name);
            LoseLife();
            
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            AddLife();
        }

        bool isGrounded = movementScript.isGrounded;

        // Kontrollera om vi just landade och tog fallskada
        if (isGrounded && !wasGroundedLastFrame)
        {
            if (lastYVelocity < fallLimit)
            {
                Debug.Log("Tog fallskada! Hastighet: " + lastYVelocity);
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

        // Testknapp för att ta skada
        if (Input.GetKeyDown(KeyCode.H)) LoseLife();
    }

    public void LoseLife()
    {
        if (isInvincible) return;

        CameraFollow.Instance.TriggerShake(0.15f, 0.2f);

        currentLives--;
        Debug.Log("Player lost a life! Lives left: " + currentLives);
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
            Debug.Log("Player gained a life! Max lives now: " + maxLives);
            
        }
        if (currentLives < maxLives)
        {
            currentLives++;
            Debug.Log("Player gained a life! Lives now: " + currentLives);
            
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
    public InventoryManager inventoryManager;
    void Die()
    {
        Debug.Log("Player is dead!");
        if (maxLives == 1) gameOver.gameObject.SetActive(true);
        else death.gameObject.SetActive(true);
        if (inventoryManager != null)
        {
            inventoryManager.DropWeaponOnDeath();
        }
        Time.timeScale = 0; // Stoppa spelet
    }

    public void SuperRespawn()
    {
        Time.timeScale = 1;
        PlayerRespawn.Instance.RespawnAtSuperCheckpoint();
        maxLives = 3; // Sätt tillbaka till max liv
        currentLives = maxLives;
        UpdateHearts();
        gameOver.gameObject.SetActive(false);
        isInvincible = true;
        invincibilityTimer = invincibilityDuration * 2;
        Debug.Log("Player respawned at super checkpoint!");
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

        Debug.Log("Player respawned!");
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
}
