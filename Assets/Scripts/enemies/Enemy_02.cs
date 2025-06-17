using UnityEngine;

public class Enemy_02 : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float projectileSpeed = 10f;

    private float timer = 0f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found in scene!");
        }
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            ShootAtPlayer();
            timer = 0f;
        }
    }

    void ShootAtPlayer()
    {
        Vector3 direction = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile02 projScript = proj.GetComponent<projectile02>();
        projScript.SetDirection(direction);
        projScript.speed = projectileSpeed;
    }
}
