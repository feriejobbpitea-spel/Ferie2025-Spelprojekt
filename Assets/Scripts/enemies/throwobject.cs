using UnityEditor;
using UnityEngine;
public class throwobject : MonoBehaviour
{
    public float lifeTime = 5f;
    private Rigidbody2D rb;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (rb != null && rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 180f); // Spegelvändning
        }
    }
}






