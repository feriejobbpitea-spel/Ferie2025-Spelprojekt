using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public GameObject hitArea;              // HitArea-objektet med t.ex. BoxCollider2D
    public LayerMask enemyLayer;
    public int damage = 10;
    public float attackCooldown = 1f;

    private float cooldownTimer = 0f;
    private Collider2D hitCollider;

    void Start()
    {
        hitCollider = hitArea.GetComponent<Collider2D>();
        transform.localPosition = Vector3.zero; // Sätt vapnets position relativt spelaren
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        // Aktivera för att kunna köra overlap (om det är inaktivt)
        hitArea.SetActive(true);

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = enemyLayer;
        filter.useLayerMask = true;

        Collider2D[] results = new Collider2D[10];
        int count = hitCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            EnemyHealth enemy = results[i].GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // Valfritt: slå av träffytan igen efter attack
        hitArea.SetActive(false);
    }
}
