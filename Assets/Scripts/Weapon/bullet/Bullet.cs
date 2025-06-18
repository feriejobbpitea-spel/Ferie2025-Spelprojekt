using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 2f;
    public float speed = 10f;
    public float radius = 0.1f; // Radius of the cast for hit detection
    public LayerMask CollisionIgnore;

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
        direction = transform.right; // Assuming bullet faces right
    }

    void Update()
    {
        float distance = speed * Time.deltaTime;
        Vector2 origin = transform.position;

        // Perform a CircleCast to detect collisions
        RaycastHit2D hit = Physics2D.CircleCast(origin, radius, direction, distance, ~CollisionIgnore);

        if (hit.collider != null)
        {
            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("Enemy"))
            {
                EnemyHealth enemy = target.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }

            Destroy(gameObject);
        }
        else
        {
            // No hit — move forward
            transform.Translate(direction * distance, Space.World);
        }
    }
}
