using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public int maxLives = 3;
    private int currentLives;

    public Image[] hearts;         // Dra in tre Image-objekt från Canvas
    public Sprite fullHeartR;      // Rött hjärta (för hjärta 0)
    public Sprite fullHeartG;      // Grönt hjärta (för hjärta 1)
    public Sprite fullHeartB;      // Blått hjärta (för hjärta 2)
    public Sprite emptyHeart;      // Grått/tomt hjärta
    public Image death;
    public Transform respawnPoint; // Om du vill ha en respawnpunkt

    private bool isInvincible = false;
    public float invincibilityDuration = 1f; // hur länge man är odödlig
    private float invincibilityTimer;



    void Start()
    {
        currentLives = maxLives;
        UpdateHearts();
        Debug.Log("Player lives: " + currentLives);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }

    public void LoseLife()
    {
        if (isInvincible) return; // Om vi är odödlig, ta inte skada

        currentLives--;
        Debug.Log("Player lost a life! Lives left: " + currentLives);
        UpdateHearts();
        // Aktivera odödlighet
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        if (currentLives <= 0)
        {
            Die();
        }
    }


    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentLives)
            {
                // Välj rätt färg på fullt hjärta beroende på index
                switch (i)
                {
                    case 0: hearts[i].sprite = fullHeartR; break;
                    case 1: hearts[i].sprite = fullHeartG; break;
                    case 2: hearts[i].sprite = fullHeartB; break;
                }
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
        death.gameObject.SetActive(true);
        // Ladda om scenen eller visa Game Overd a
        Time.timeScale = 0; // Stoppa spelet
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
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

        // Återställ liv till 1
        currentLives = maxLives;
        UpdateHearts();  // Uppdatera UI

        // Aktivera spelaren igen om den var död/inaktiv
        death.gameObject.SetActive(false);

        Debug.Log("Player respawned!");
    }
}
