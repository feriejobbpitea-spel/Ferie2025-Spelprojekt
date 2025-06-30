using UnityEngine;

public class ConfettiLauncher : MonoBehaviour
{
    public GameObject confettiPrefab;
    public Transform shootPoint;
    public float shootForce = 15f;
    public float cooldownTime = 1f;

    // Statisk cooldown-timer, delas mellan ALLA ConfettiLauncher-instans
    private static float globalCooldownTimer = 0f;

    void Update()
    {
        // Minska den globala cooldown-timern (Time.deltaTime gäller globalt)
        if (globalCooldownTimer > 0f)
            globalCooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && globalCooldownTimer <= 0f)
        {
            Shoot();
            globalCooldownTimer = cooldownTime; // Starta cooldown globalt
        }
    }

    void Shoot()
    {
        Debug.Log("Shooting confetti!");
        GameObject bullet = Instantiate(confettiPrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * shootForce;

        // Om du vill ignorera collision mellan skott och spelare kan du aktivera detta:
        /*
        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        Collider2D myCollider = GetComponent<Collider2D>();
        if (bulletCollider != null && myCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, myCollider);
        }
        */
    }
}
