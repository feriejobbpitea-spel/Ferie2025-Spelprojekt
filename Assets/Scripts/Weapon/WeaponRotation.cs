using UnityEngine;

public class WeaponRotation : MonoBehaviour
{
    public Transform player;              // Spelarens transform
    public float offset = 0.05f;          // Vapnets offset från spelarens mitt
    public GameObject bulletPrefab;       // Din bullet-prefab
    public Transform firePoint;           // Punkt där kulan skapas
    public float fireRate = 0.2f;         // Tid mellan skott

    private float fireCooldown = 0f;

    void Start()
    {
      player = GameObject.FindGameObjectWithTag("Player")?.transform;
      
        transform.localPosition = Vector3.zero; // Sätt vapnets position relativt spelaren
    }
    void Update()
    {
        RotateAndPositionWeapon();
        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate;
        }
    }

    private void OnValidate()
    {
        transform.localPosition = Vector3.zero;
    }

    void RotateAndPositionWeapon()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Sätt vapnet till vänster/höger sida av spelaren
        bool onRight = mousePos.x >= player.position.x;
        Vector3 offsetPos = new Vector3(onRight ? offset : -offset, 0, 0);
        transform.position = player.position + offsetPos;

        // Räkna ut riktning till musen
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Roterar vapnet mot musen
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flippa på Y-led om vi är till vänster
        transform.localScale = new Vector3(1, onRight ? 1 : -1, 1);
    }

    void Shoot()
    {
        if (bulletPrefab != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
       
    }
}
