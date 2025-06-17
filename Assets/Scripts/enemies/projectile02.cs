using UnityEngine;

public class projectile02 : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
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
}