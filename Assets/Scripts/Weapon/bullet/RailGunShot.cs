using UnityEngine;
using UnityEngine.UI;

public class RailgunShot : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firePoint;
    public LayerMask hitMask;
    
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
        Vector3 direction = firePoint.right.normalized;

        // Skärmens hörn i världskordinater
        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        float maxDistance = 10000f; // väldigt stort värde som fallback

        // Lista med möjliga avstånd till kanter i riktningen
        System.Collections.Generic.List<float> distances = new System.Collections.Generic.List<float>();

        // Funktion för att räkna ut avstånd längs ray mot en given x eller y gräns
        float DistanceToLine(float linePos, bool isVertical)
        {
            if (isVertical)
            {
                // x = linePos, räkna t så att start.x + t*direction.x = linePos
                if (Mathf.Approximately(direction.x, 0)) return -1f;
                float t = (linePos - start.x) / direction.x;
                if (t < 0) return -1f; // bakom start
                return t;
            }
            else
            {
                // y = linePos, räkna t så att start.y + t*direction.y = linePos
                if (Mathf.Approximately(direction.y, 0)) return -1f;
                float t = (linePos - start.y) / direction.y;
                if (t < 0) return -1f;
                return t;
            }
        }

        // Kolla mot vänster och höger skärmgränser
        float distLeft = DistanceToLine(screenBottomLeft.x, true);
        if (distLeft > 0) distances.Add(distLeft);

        float distRight = DistanceToLine(screenTopRight.x, true);
        if (distRight > 0) distances.Add(distRight);

        // Kolla mot botten och topp skärmgränser
        float distBottom = DistanceToLine(screenBottomLeft.y, false);
        if (distBottom > 0) distances.Add(distBottom);

        float distTop = DistanceToLine(screenTopRight.y, false);
        if (distTop > 0) distances.Add(distTop);

        if (distances.Count > 0)
        {
            maxDistance = Mathf.Min(distances.ToArray()); // närmaste avstånd till skärmkant i riktningen
        }

         // lägg på lite extra räckvidd

        RaycastHit2D hit = Physics2D.Raycast(start, direction, maxDistance, hitMask);

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
            lineRenderer.SetPosition(1, start + direction * maxDistance);
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
