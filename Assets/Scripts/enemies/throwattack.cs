using UnityEngine;
using System.Collections;

public class throwattack : MonoBehaviour
{
    public Transform player;                 // Spelarens transform
    public Transform throwPoint;            // Varifr�n f�rem�let kastas
    public GameObject throwablePrefab;      // Prefab f�r det kastbara f�rem�let
    public float throwForce = 10f;
    public float attackCooldown = 3f;

    private bool isAttacking = false;
    private float cooldownTimer;

    private void Start()
    {
        StartCoroutine(PerformThrowAttack());
    }

    private IEnumerator PerformThrowAttack()
    {
        while (this.enabled)
        {
            yield return new WaitForSeconds(0.5f);

            // Kasta f�rem�l
            Vector3 direction = (player.position - throwPoint.position).normalized;

            GameObject thrownObj = Instantiate(throwablePrefab, throwPoint.position, Quaternion.identity);
            Rigidbody2D rb = thrownObj.GetComponent<Rigidbody2D>();
            rb.linearVelocity = direction * throwForce;

            yield return new WaitForSeconds(1f);
        }
    }
}
