using UnityEngine;
using UnityEngine.UI;

public class RailgunShot : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firePoint;
    public LayerMask hitMask;
    public float range = 20f;
    public float damagePerSecond = 10f;

    [Header("Energy System")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyDrainPerSecond = 20f;
    public float energyRegenPerSecond = 15f;
    public Slider energySlider;

    private float damageBuffer = 0f;
    private bool isFiring = false;

    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateSlider();
    }

    void Update()
    {
        bool inputFire = Input.GetMouseButton(0);
        bool canFire = inputFire && Time.timeScale > 0 && currentEnergy > 0f;

        if (canFire)
        {
            isFiring = true;
            FireLaser();
            DrainEnergy();
        }
        else
        {
            if (isFiring)
            {
                lineRenderer.enabled = false;
                damageBuffer = 0f;
                isFiring = false;
            }

            // Endast regenerera om inte skjutning pågår
            if (currentEnergy < maxEnergy && !inputFire)
            {
                currentEnergy += energyRegenPerSecond * Time.deltaTime;
                currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
                UpdateSlider();
            }
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
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, start + direction * range);
        }
    }

    void DrainEnergy()
    {
        currentEnergy -= energyDrainPerSecond * Time.deltaTime;
        currentEnergy = Mathf.Max(currentEnergy, 0f);
        UpdateSlider();
    }

    void UpdateSlider()
    {
        if (energySlider != null)
            energySlider.value = currentEnergy;
    }
}
