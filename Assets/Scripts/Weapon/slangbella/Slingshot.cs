using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public GameObject stonePrefab;
    public Transform weaponTip;
    public float minForce = 5f;
    public float maxForce = 20f;
    public float chargeRate = 10f;

    public LineRenderer trajectoryLine; // Dra in i Inspector
    public int linePoints = 30;
    public float timeStep = 0.05f;

    private float currentForce = 0f;
    private bool isCharging = false;

    void Start()
    {
        if (trajectoryLine != null)
        {
            trajectoryLine.textureMode = LineTextureMode.Tile;

            // Sätt transparent svart färg på linjen
            trajectoryLine.startColor = new Color(0f, 0f, 0f, 0.5f);
            trajectoryLine.endColor = new Color(0f, 0f, 0f, 0.5f);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentForce = minForce;
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            currentForce += chargeRate * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, minForce, maxForce);
            UpdateTrajectoryLine(currentForce);
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            Shoot(currentForce);
            isCharging = false;
            trajectoryLine.positionCount = 0; // Ta bort linjen efter skott
        }
    }

    void Shoot(float force)
    {
        GameObject newStone = Instantiate(stonePrefab, weaponTip.position, Quaternion.identity);
        Rigidbody2D rb = newStone.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 shootDirection = weaponTip.right;
            rb.linearVelocity = shootDirection * force;
        }
    }

    void UpdateTrajectoryLine(float force)
    {
        Camera cam = Camera.main;
        Vector3 screenBottomLeft = cam.ScreenToWorldPoint(Vector3.zero);
        Vector3 screenTopRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        Vector2 startPos = weaponTip.position;
        Vector2 startVel = weaponTip.right * force;

        trajectoryLine.positionCount = linePoints;

        for (int i = 0; i < linePoints; i++)
        {
            float t = i * timeStep;
            Vector2 point = startPos + startVel * t + 0.5f * Physics2D.gravity * t * t;

            // Begränsa punkter inom kamerans synfält
            point.x = Mathf.Clamp(point.x, screenBottomLeft.x, screenTopRight.x);
            point.y = Mathf.Clamp(point.y, screenBottomLeft.y, screenTopRight.y);

            trajectoryLine.SetPosition(i, point);
        }

        // Justera strecktäthet baserat på kraften
        if (trajectoryLine.material != null)
            trajectoryLine.material.mainTextureScale = new Vector2(force, 1);
    }
}
