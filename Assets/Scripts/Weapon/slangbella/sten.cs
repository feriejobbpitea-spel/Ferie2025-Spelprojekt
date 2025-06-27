using UnityEngine;

public class Sten : MonoBehaviour
{
    public float lifeTime = 5f;
    public float baseDamage = 10f;
    public float damageMultiplier = 1.5f; // Skala på hur mycket skada baseras på träffkraft

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D saknas på stenen!");
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
            enemyHealth = collision.collider.GetComponentInChildren<EnemyHealth>();
        }

            if (enemyHealth != null)
        {
            float impactForce = collision.relativeVelocity.magnitude;
            int totalDamage = Mathf.RoundToInt(baseDamage * impactForce * damageMultiplier);

            Debug.Log($"Ger {totalDamage} skada (base {baseDamage} + kraft {impactForce:F2}) till {collision.collider.name}");
            enemyHealth.TakeDamage(totalDamage);
            Destroy(gameObject);
            return;
        }

        // Om träffar något annat (vägg/mark)
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Debug.Log("Stenen lämnade skärmen och tas bort.");
        Destroy(gameObject);
    }
}
