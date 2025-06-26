using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour
{
    public GameObject hitArea;
    public LayerMask enemyLayer;
    public int damage = 10;
    public float attackCooldown = 1f;
    public float attackAnimationDuration = 0.5f;

    public Animator playerAnimator;  // Animator på spelaren

    private float cooldownTimer = 0f;
    private Collider2D hitCollider;

    void Start()
    {
        hitCollider = hitArea.GetComponentInChildren<Collider2D>();
        playerAnimator =  transform.parent.GetComponentInChildren<Animator>();

        if (!hitCollider.isTrigger)
        {
            Debug.LogWarning("HitArea collider måste vara Is Trigger för att inte putta fiender!");
        }
        hitArea.SetActive(false);

        if (playerAnimator != null)
        {
            playerAnimator.SetLayerWeight(1, 0f); // Layer off i början
        }
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
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isAttacking", true);
            playerAnimator.SetLayerWeight(1, 1f);
        }

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

        StartCoroutine(EndAttackAfterDelay());
    }


    IEnumerator EndAttackAfterDelay()
    {
        yield return new WaitForSeconds(attackAnimationDuration);

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isAttacking", false);
            playerAnimator.SetLayerWeight(1, 0f);
        }

        hitArea.SetActive(false);
    }
}
