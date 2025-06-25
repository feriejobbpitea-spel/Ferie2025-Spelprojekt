using UnityEngine;

public class Sten : MonoBehaviour
{
    public float lifeTime = 5f;
    public int damage = 10;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D saknas p� stenen!");
        }
        else
        {
            Debug.Log("Stenen startar med velocity: " + rb.linearVelocity);
        }

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Sten krockade med {collision.collider.name}");

        EnemyHealth enemyHealth = collision.collider.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            Debug.Log($"Ger {damage} skada till {collision.collider.name}");
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Om det �r n�got annat (t.ex. mark eller v�gg), f�rst�r stenen �nd�
        Destroy(gameObject);
    }
}
