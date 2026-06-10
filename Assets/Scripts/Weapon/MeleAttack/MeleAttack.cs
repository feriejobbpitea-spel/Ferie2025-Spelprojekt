using System;
using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject hitArea;
    public LayerMask enemyLayer;
    public int damage = 10;
    public float attackCooldown = 1f;
    public float attackAnimationDuration = 0.5f;

    [Header("Components")]
    public Animator playerAnimator;  // Animator på spelaren
    public AudioClip swingSound;     // Ljudklipp för sving
    private AudioSource audioSource; // AudioSource-komponent

    private float cooldownTimer = 0f;
    private Collider2D hitCollider;
    public GameObject airAttackEffectPrefab;

    void Start()
    {
        hitCollider = hitArea.GetComponentInChildren<Collider2D>();
        playerAnimator = transform.parent.GetComponentInChildren<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource saknas på " + gameObject.name + ", lägger till en automatiskt.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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

        if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Shoot", KeyCode.Mouse0.ToString()))) && cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        GameObject.Instantiate(airAttackEffectPrefab, transform.position +new Vector3(1.2f,0), Quaternion.identity);
        GameObject effect = GameObject.Instantiate(
     airAttackEffectPrefab,
     transform.position - new Vector3(1.2f, 0),
     Quaternion.identity // Ingen rotation
            );
        Vector3 scale = effect.transform.localScale;
        scale.x = -Mathf.Abs(scale.x); // Se till att det blir spegelvänt åt vänster
        effect.transform.localScale = scale;





        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isAttacking", true);
            playerAnimator.SetLayerWeight(1, 1f);
        }

        // Spela svingljudet
        if (swingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(swingSound);
        }

        hitArea.SetActive(true);

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = enemyLayer;
        filter.useLayerMask = true;

        Collider2D[] results = new Collider2D[10];
        int count = hitCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Hit {results[i].name} with MeleeAttack");

            EnemyHealth enemyHealth = results[i].GetComponent<EnemyHealth>();
            if (enemyHealth == null)
            {
                enemyHealth = results[i].GetComponentInChildren<EnemyHealth>();
            }
            enemyHealth?.TakeDamage(damage);
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