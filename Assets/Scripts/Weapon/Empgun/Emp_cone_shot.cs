using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConeBeamShot : MonoBehaviour
{
    public Transform firePoint;
    public float maxLength = 4f;
    public float maxAngle = 30f;
    public float growSpeed = 10f;
    public float moveSpeed = 8f;
    public float shrinkSpeed = 12f;

    [Header("Charge UI")]
    public Slider chargeSlider;
    public TextMeshProUGUI chargeText;
    public Image sliderFillImage;

    public Color normalColor = Color.white;
    public Color blinkColor = Color.blue;
    public float blinkSpeed = 2f;  // Speed of fade cycle

    private Mesh mesh;
    private float currentStart = 0f;
    private float currentEnd = 0f;
    private bool isShooting = false;

    private Vector3 shootDirection;

    private float currentCharge = 1f;
    private float maxCharge = 1f;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();

        if (chargeSlider != null)
        {
            chargeSlider.minValue = 0f;
            chargeSlider.maxValue = 1f;
            chargeSlider.value = currentCharge;
        }
        if (sliderFillImage != null)
            sliderFillImage.color = normalColor;

        UpdateChargeUI();
    }

    void Update()
    {
        if (!isShooting)
            currentCharge = Mathf.Min(currentCharge, maxCharge);

        currentCharge += Time.deltaTime * 0.5f;

        UpdateChargeUI();

        // Färga slidern med smooth blink om laddad
        if (currentCharge >= maxCharge)
        {
            if (sliderFillImage != null)
            {
                float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
                sliderFillImage.color = Color.Lerp(normalColor, blinkColor, t);
            }
        }
        else
        {
            if (sliderFillImage != null)
                sliderFillImage.color = normalColor;
        }

        // Skjut
        if (Input.GetMouseButtonDown(0) && currentCharge >= maxCharge && !isShooting)
        {
            Shoot();
        }

        // Strål-animation
        if (isShooting)
        {
            if (currentEnd < maxLength)
            {
                currentEnd += growSpeed * Time.deltaTime;
                if (currentEnd > maxLength)
                    currentEnd = maxLength;
            }
            else
            {
                transform.position += shootDirection * moveSpeed * Time.deltaTime;
                currentStart += shrinkSpeed * Time.deltaTime;
                currentEnd += moveSpeed * Time.deltaTime;

                if (currentStart >= currentEnd)
                {
                    isShooting = false;
                    mesh.Clear();
                    return;
                }
            }

            UpdateMesh();
            CheckHitEnemies();
        }
    }

    void Shoot()
    {
        isShooting = true;
        currentCharge = 0f;

        currentStart = 0f;
        currentEnd = 0f;
        transform.position = firePoint.position;
        transform.parent = null;
        transform.rotation = firePoint.rotation;
        shootDirection = firePoint.right.normalized;
        sliderFillImage.color = normalColor;
    }

    void UpdateChargeUI()
    {
        if (chargeSlider != null)
            chargeSlider.value = currentCharge / maxCharge;

        if (chargeText != null)
        {
            if (currentCharge >= maxCharge)
            {
                chargeText.text = "Ready";
                chargeText.fontSize = 40;
                chargeText.rectTransform.anchoredPosition = new Vector2(30, -235);
            }
            else
            {
                chargeText.text = $"{(currentCharge / maxCharge) * 100f:0}%";
                chargeText.fontSize = 50;
                chargeText.rectTransform.anchoredPosition = new Vector2(45, -225);
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        float halfAngleRad = Mathf.Deg2Rad * maxAngle / 2f;

        Vector3 topLeft = new Vector3(currentStart, Mathf.Tan(halfAngleRad) * currentStart, 0);
        Vector3 topRight = new Vector3(currentEnd, Mathf.Tan(halfAngleRad) * currentEnd, 0);
        Vector3 bottomLeft = new Vector3(currentStart, -Mathf.Tan(halfAngleRad) * currentStart, 0);
        Vector3 bottomRight = new Vector3(currentEnd, -Mathf.Tan(halfAngleRad) * currentEnd, 0);

        Vector3[] vertices = new Vector3[4]
        {
            topLeft, topRight, bottomLeft, bottomRight
        };

        int[] triangles = new int[6]
        {
            0, 1, 2, 2, 1, 3
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void CheckHitEnemies()
    {
        float halfAngleRad = Mathf.Deg2Rad * maxAngle / 2f;
        float beamWidth = Mathf.Tan(halfAngleRad) * currentEnd;
        Vector2 center = transform.position + shootDirection * (currentStart + currentEnd) / 2f;
        Vector2 size = new Vector2(currentEnd - currentStart, beamWidth * 2f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, transform.eulerAngles.z, LayerMask.GetMask("Enemies"));

        foreach (Collider2D hit in hits)
        {
            // Kolla om det är Enemy_01
            Enemy_01 enemy1 = hit.GetComponent<Enemy_01>();
            if (enemy1 != null)
            {
                enemy1.Stun(10f);
                continue; // Gå till nästa collider, ingen anledning att kolla Enemy_02 om det redan är Enemy_01
            }

            // Kolla om det är Enemy_02
            Enemy_02 enemy2 = hit.GetComponent<Enemy_02>();
            if (enemy2 != null)
            {
                enemy2.Stun(50f);
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !isShooting) return;

        float halfAngleRad = Mathf.Deg2Rad * maxAngle / 2f;
        float beamWidth = Mathf.Tan(halfAngleRad) * currentEnd;
        Vector2 center = transform.position + shootDirection * (currentStart + currentEnd) / 2f;
        Vector2 size = new Vector2(currentEnd - currentStart, beamWidth * 2f);

        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, size);
    }
}
