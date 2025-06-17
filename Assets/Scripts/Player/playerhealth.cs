using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public int maxLives = 3;
    private int currentLives;

    public Image[] hearts;         // Dra in tre Image-objekt fr�n Canvas
    public Sprite fullHeartR;      // R�tt hj�rta (f�r hj�rta 0)
    public Sprite fullHeartG;      // Gr�nt hj�rta (f�r hj�rta 1)
    public Sprite fullHeartB;      // Bl�tt hj�rta (f�r hj�rta 2)
    public Sprite emptyHeart;      // Gr�tt/tomt hj�rta
    public Image death;
    public Transform respawnPoint; // Om du vill ha en respawnpunkt

    private bool isInvincible = false;
    public float invincibilityDuration = 1f; // hur l�nge man �r od�dlig
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
        if (isInvincible) return; // Om vi �r od�dlig, ta inte skada

        currentLives--;
        Debug.Log("Player lost a life! Lives left: " + currentLives);
        UpdateHearts();
        // Aktivera od�dlighet
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
                // V�lj r�tt f�rg p� fullt hj�rta beroende p� index
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
        // Du kan l�gga till testknappar h�r:
        if (Input.GetKeyDown(KeyCode.H)) LoseLife();
    }
    public void Respawn()
    {
        Time.timeScale = 1;
        // Flytta spelaren till spawnpoint
        PlayerRespawn.Instance.Respawn();

        // �terst�ll liv till 1
        currentLives = maxLives;
        UpdateHearts();  // Uppdatera UI

        // Aktivera spelaren igen om den var d�d/inaktiv
        death.gameObject.SetActive(false);

        Debug.Log("Player respawned!");
    }
}
