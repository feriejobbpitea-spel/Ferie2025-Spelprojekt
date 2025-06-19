using UnityEngine;

public class RailgunShot : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firePoint;               // Vapnets spets
    public LayerMask hitMask;
    public float range = 20f;

    public float damagePerSecond = 10f;
    private float damageBuffer = 0f;

    private bool isFiring = false;

  
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            isFiring = true;
            FireLaser();
        }
        else
        {
            isFiring = false;
            lineRenderer.enabled = false;
            damageBuffer = 0f; // Valfritt: nollställ om du vill att buffern inte hänger kvar
        }
    }

    void FireLaser()
    {
        lineRenderer.enabled = true;

        Vector3 start = firePoint.position;
        Vector3 direction = firePoint.right;

        RaycastHit2D hit = Physics2D.Raycast(start, direction, range, hitMask);

        if (hit.collider != null)
        {
            Vector3 hitPoint = hit.point;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, hitPoint);

            // Skada fienden
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                damageBuffer += damagePerSecond * Time.deltaTime;
                if (damageBuffer >= 1f)
                {
                    int damageToApply = Mathf.FloorToInt(damageBuffer);
                    enemy.TakeDamage(damageToApply);
                    damageBuffer -= damageToApply;
                }
            }
        }
        else
        {
            // Ingen träff – skjut full längd
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, start + direction * range);
        }
    }
}
