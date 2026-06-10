using System;
using UnityEngine;

public class ConfettiLauncher : MonoBehaviour
{
    public GameObject confettiPrefab;
    public Transform shootPoint;
    public float shootForce = 15f;
    public float cooldownTime = 1f;

    public AudioClip shootLoopSound; // Loopande ljud under färd
    public AudioClip explosionSound;  // Ljud vid explosion

    // Statisk cooldown-timer, delas mellan alla instanser
    private static float globalCooldownTimer = 0f;

    void Update()
    {
        if (globalCooldownTimer > 0f)
            globalCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Shoot", KeyCode.Mouse0.ToString()))) && globalCooldownTimer <= 0f)
        {
            Shoot();
            globalCooldownTimer = cooldownTime;
        }
    }

    void Shoot()
    {
        Debug.Log("Shooting confetti!");
        GameObject bullet = Instantiate(confettiPrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.right * shootForce;
        }

        ConfettiBall projectile = bullet.GetComponent<ConfettiBall>();
        if (projectile != null)
        {
            projectile.shootLoopSound = shootLoopSound;
            projectile.explosionSound = explosionSound;
        }
    }
}
