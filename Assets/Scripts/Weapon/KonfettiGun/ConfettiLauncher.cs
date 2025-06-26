using UnityEngine;

public class ConfettiLauncher : MonoBehaviour
{
    public GameObject confettiPrefab;   // Prefab f�r konfetti-bollen
    public Transform shootPoint;        // Position d�r bollen skjuts fr�n
    public float shootForce = 15f;      // Hur snabbt bollen skjuts iv�g
    public float cooldownTime = 1f;

    private float cooldownTimer = 0f;

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && cooldownTimer <= 0f)
        {
            Shoot();
            cooldownTimer = cooldownTime;
        }
    }

    void Shoot()
    {
        Debug.Log("Shooting confetti!"); // Debug-utskrift f�r att bekr�fta skottet
        GameObject bullet = Instantiate(confettiPrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = (transform.right * shootForce);

        // Ignorera collision mellan bollen och den h�r (launchern/spelaren)
/*        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        Collider2D myCollider = GetComponent<Collider2D>();
        if (bulletCollider != null && myCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, myCollider);
        }*/
    }

}
