using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Emp_cone_shots : MonoBehaviour
{
    public float maxLength = 0.5f;
    public float maxAngle = 30f;
    public float growSpeed = 100f;
    public float moveSpeed = 5f;
    public float shrinkSpeed = 5.5f;

    private Mesh mesh;
    private float currentStart = 0f;
    private float currentEnd = 0f;
    private bool isShooting = true;  // Sätt till true direkt, beam skapas redan när skott skjuts

    private Vector3 shootDirection;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();

        shootDirection = transform.right.normalized;
    }

    void Update()
    {
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

                    Destroy(gameObject);
                    return;
                }
            }

            UpdateMesh();
            CheckHitEnemies();
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

        Vector3[] vertices = new Vector3[4] { topLeft, topRight, bottomLeft, bottomRight };
        int[] triangles = new int[6] { 0, 1, 2, 2, 1, 3 };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void CheckHitEnemies()
    {
        float halfAngleRad = Mathf.Deg2Rad * maxAngle / 2f;
        float beamWidth = Mathf.Tan(halfAngleRad) * currentEnd;
        Vector2 center = (Vector2)(transform.position + shootDirection * (currentStart + currentEnd) / 2f);
        Vector2 size = new Vector2(currentEnd - currentStart, beamWidth * 2f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, transform.eulerAngles.z, LayerMask.GetMask("Enemies"));

        foreach (Collider2D hit in hits)
        {
            Enemy_01 enemy1 = hit.GetComponent<Enemy_01>();
            if (enemy1 != null)
            {
                enemy1.Stun(5f);
                continue;
            }

            Enemy_02 enemy2 = hit.GetComponent<Enemy_02>();
            if (enemy2 != null)
            {
                enemy2.Stun(20f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !isShooting) return;

        float halfAngleRad = Mathf.Deg2Rad * maxAngle / 2f;
        float beamWidth = Mathf.Tan(halfAngleRad) * currentEnd;
        Vector2 center = (Vector2)(transform.position + shootDirection * (currentStart + currentEnd) / 2f);
        Vector2 size = new Vector2(currentEnd - currentStart, beamWidth * 2f);

        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, size);
    }
}
