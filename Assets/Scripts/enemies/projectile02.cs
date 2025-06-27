using UnityEngine;

public class projectile02 : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        // ROTERA PROJEKTILEN I Z-LED SÅ ATT DEN "PEKAR" I RIKTNINGEN
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("goTrough"))
        {
            Destroy(gameObject, 0.1F);
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealthV2>().LoseLife();
            Destroy(gameObject, 0.1f);
        }
    }
}
