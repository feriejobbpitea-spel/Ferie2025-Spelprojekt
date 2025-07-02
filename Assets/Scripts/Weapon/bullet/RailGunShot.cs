using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RailgunShot : MonoBehaviour
{
    public static float globalCurrentEnergy;
    public static float globalMaxEnergy = 100f;
    public static float globalCooldownTimer = 0f;

    public float globalCooldownTime = 1f;  // cooldown efter skjutning

    public LineRenderer lineRenderer;
    public Transform firePoint;
    public LayerMask hitMask;

    public float damagePerSecond = 10f;
    public float energyDrainPerSecond = 20f;
    public float energyRegenPerSecond = 15f;
    public Slider energySlider;

    public AudioSource shootingAudioSource;  // Ljudkomponenten

    private float damageBuffer = 0f;
    private bool isFiring = false;

    void Start()
    {
        if (globalCurrentEnergy <= 0f)
            globalCurrentEnergy = globalMaxEnergy;

        UpdateSlider();

        if (shootingAudioSource != null)
        {
            shootingAudioSource.loop = true; // Loopande ljud så länge man skjuter
            shootingAudioSource.Stop();      // Se till att ljudet inte spelar från start
        }
    }

    void Update()
    {
        if (globalCooldownTimer > 0f)
            globalCooldownTimer -= Time.deltaTime;

        bool inputFire = Input.GetMouseButton(0);
        bool canFire = inputFire && Time.timeScale > 0 && globalCurrentEnergy > 0f && globalCooldownTimer <= 0f;

        if (canFire)
        {
            if (!isFiring)
            {
                isFiring = true;
                if (shootingAudioSource != null && !shootingAudioSource.isPlaying)
                {
                    shootingAudioSource.Play(); // Starta ljudet när man börjar skjuta
                }
            }

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

                // Stoppa ljudet direkt när man slutar skjuta
                if (shootingAudioSource != null && shootingAudioSource.isPlaying)
                {
                    shootingAudioSource.Stop();
                }

                globalCooldownTimer = globalCooldownTime;
            }

            if (globalCurrentEnergy < globalMaxEnergy && !inputFire)
            {
                globalCurrentEnergy += energyRegenPerSecond * Time.deltaTime;
                globalCurrentEnergy = Mathf.Min(globalCurrentEnergy, globalMaxEnergy);
                UpdateSlider();
            }
        }
    }

    void FireLaser()
    {
        // Din befintliga kod för laser-skjutning
        lineRenderer.enabled = true;

        Vector3 start = firePoint.position;
        Vector3 direction = firePoint.right.normalized;

        Vector3 screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 screenTopRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        float maxDistance = 10000f;

        System.Collections.Generic.List<float> distances = new System.Collections.Generic.List<float>();

        float DistanceToLine(float linePos, bool isVertical)
        {
            if (isVertical)
            {
                if (Mathf.Approximately(direction.x, 0)) return -1f;
                float t = (linePos - start.x) / direction.x;
                if (t < 0) return -1f;
                return t;
            }
            else
            {
                if (Mathf.Approximately(direction.y, 0)) return -1f;
                float t = (linePos - start.y) / direction.y;
                if (t < 0) return -1f;
                return t;
            }
        }

        float distLeft = DistanceToLine(screenBottomLeft.x, true);
        if (distLeft > 0) distances.Add(distLeft);

        float distRight = DistanceToLine(screenTopRight.x, true);
        if (distRight > 0) distances.Add(distRight);

        float distBottom = DistanceToLine(screenBottomLeft.y, false);
        if (distBottom > 0) distances.Add(distBottom);

        float distTop = DistanceToLine(screenTopRight.y, false);
        if (distTop > 0) distances.Add(distTop);

        if (distances.Count > 0)
        {
            maxDistance = Mathf.Min(distances.ToArray());
        }

        RaycastHit2D hit = Physics2D.Raycast(start, direction, maxDistance, hitMask);

        if (hit.collider != null)
        {
            Vector3 hitPoint = hit.point;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, hitPoint);

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy == null) { enemy = hit.collider.GetComponentInChildren<EnemyHealth>(); }
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
        globalCurrentEnergy -= energyDrainPerSecond * Time.deltaTime;
        globalCurrentEnergy = Mathf.Max(globalCurrentEnergy, 0f);
        UpdateSlider();
    }

    void UpdateSlider()
    {
        if (energySlider != null)
            energySlider.value = globalCurrentEnergy;
    }
}
