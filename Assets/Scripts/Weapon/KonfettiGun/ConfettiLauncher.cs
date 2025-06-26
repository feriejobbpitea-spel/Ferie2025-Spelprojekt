using UnityEngine;

public class ConfettiLauncher : MonoBehaviour
{
    public GameObject confettiPrefab;   // Prefab för konfetti-bollen
    public Transform shootPoint;        // Position där bollen skjuts från
    public float shootForce = 15f;      // Hur snabbt bollen skjuts iväg
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
        Debug.Log("Shooting confetti!"); // Debug-utskrift för att bekräfta skottet
        GameObject bullet = Instantiate(confettiPrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = (transform.right * shootForce);

        // Ignorera collision mellan bollen och den här (launchern/spelaren)
/*        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        Collider2D myCollider = GetComponent<Collider2D>();
        if (bulletCollider != null && myCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, myCollider);
        }*/
    }

}
