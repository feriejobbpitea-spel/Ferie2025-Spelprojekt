using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public int maxLives = 3;
    public int currentLives;

    public Image[] hearts;         // Dra in tre Image-objekt från Canvas
    public Sprite fullHeartR;      // Rött hjärta (för hjärta 0)
    public Sprite fullHeartG;      // Grönt hjärta (för hjärta 1)
    public Sprite fullHeartB;      // Blått hjärta (för hjärta 2)
    public Sprite emptyHeart;      // Grått/tomt hjärta
    public Image death;
    public Image pause;
    public Image gameOver;
    private bool isInvincible = false;
    public float invincibilityDuration = 2f; // hur länge man är odödlig
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

    private void OnCollisionStay2D(Collision2D collision)
    {   
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }
    public void LoseLife()
    {
        // Lägg till kameraskakning
        
        if (isInvincible) return; // Om vi är odödlig, ta inte skada
        CameraFollow.Instance.TriggerShake(0.15f, 0.2f);

        currentLives--;
        Debug.Log("Player lost a life! Lives left: " + currentLives);
        UpdateHearts();
        // Aktivera odödlighet
        isInvincible = true;
        invincibilityTimer = invincibilityDuration*1;

        if (currentLives <= 0)
        {
            Die();
        }
        else { StartCoroutine(FlashWhite());  }

       
    }
    public void AddLife()
    {
        
    
    
        if (maxLives < 4)
        {
            maxLives++; // Öka max liv med 1
            
            Debug.Log("Player gained a life! Lives now: " + currentLives);
            
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRespawn>().updateMaxLives(); // Uppdatera respawn max liv}
          }
        if (currentLives < maxLives)
        {
            currentLives++;
            Debug.Log("Player gained a life! Lives now: " + currentLives);

        }
        UpdateHearts(); // Uppdatera UI
    }
   
    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < maxLives); // Aktivera/deaktivera hjärtan baserat på maxLives
        }

        for (int i = 0; i < maxLives; i++)
        {
            if (i < currentLives)
            {
                GameObject player = GameObject.FindWithTag("Player");
                string biome = player.GetComponent<SetBiom>().biome;
                if (biome == "Grass") { hearts[i].sprite = fullHeartR; } // Rött hjärta för gräsbiom
                else if (biome == "mine") { hearts[i].sprite = fullHeartG; } // Grönt hjärta för skogsbiom
                else if (biome == "boss") { hearts[i].sprite = fullHeartB; } // Blått hjärta för snöbiom
                else { hearts[i].sprite = fullHeartR; } // Standardfärg om inget annat anges
                
                
                
                
                
                
                // Välj rätt färg på fullt hjärta beroende på index
               // switch (i)
               // {
                    
                //    case 0: hearts[i].sprite = fullHeartR; ; break;
                //    case 1: hearts[i].sprite = fullHeartG; ; break;
                //    case 2: hearts[i].sprite = fullHeartB; ; break;
               // }
            }
            else
            {
                hearts[i].sprite = emptyHeart;
               
            }
        }
    }

    void Die()
    {
        Debug.Log("Player is dead!");
        if(maxLives == 1) { gameOver.gameObject.SetActive(true); } else { death.gameObject.SetActive(true); }
            
        // Ladda om scenen eller visa Game Overd a
        Time.timeScale = 0; // Stoppa spelet
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddLife();
        }
        bool isGrounded = movementScript.isGrounded;
       
        // Kontrollera om vi just landade
        if (isGrounded && !wasGroundedLastFrame)
        {
            if (lastYVelocity < fallLimit)
            {
                Debug.Log("Tog fallskada!" + lastYVelocity);
                LoseLife(); // eller vad din metod för skada heter
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

                if (toggleWhite)
                    spriteRenderer.color = Color.white;
                else
                    spriteRenderer.color = Color.clear; // tillfälligt osynlig (kan bytas till original)
            }

            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                spriteRenderer.color = Color.white; // eller Color full opacity
            }
        }

        // Du kan lägga till testknappar här:
        if (Input.GetKeyDown(KeyCode.H)) LoseLife();
    }
    public void Respawn()
    {
        Time.timeScale = 1;
        // Flytta spelaren till spawnpoint
        PlayerRespawn.Instance.Respawn();
        maxLives--; // Minska max liv med 1
        // Återställ liv till 1
        
        currentLives = maxLives;
        UpdateHearts();  // Uppdatera UI

        // Aktivera spelaren igen om den var död/inaktiv
        death.gameObject.SetActive(false);
        isInvincible = true;
        invincibilityTimer = invincibilityDuration*2;
        
        Debug.Log("Player respawned!");
    }
    private IEnumerator FlashWhite()
    {
        spriteRenderer.material = whiteFlashMaterial;
        yield return new WaitForSeconds(0.1f);  // hur länge du ska blinka
        spriteRenderer.material = originalMaterial;
    }
    
    public void tryAgain()
    {
                Time.timeScale = 1; // Återställ tidsskalan
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Ladda om nuvarande scen
        gameOver.gameObject.SetActive(false); // Stäng av Game Over UI
    }
}
