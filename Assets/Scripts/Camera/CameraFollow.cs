using UnityEngine;
using System.Collections;

public class CameraFollow : Singleton<CameraFollow>
{
    private Transform playerTarget;
    public Transform OverrideTarget { get; set; }
    public float OverrideZoom { get; set; } = -1f;  // -1 means no override

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private Camera cam;
    private float defaultZoom;

    private void Awake()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        cam = GetComponent<Camera>();
        if (cam == null)
            Debug.LogError("CameraFollow requires a Camera component on the same GameObject!");

        if (cam != null)
        {
            defaultZoom = cam.orthographicSize; // assuming 2D orthographic camera
        }
    }

    private void Start()
    {
        if(playerTarget != null)
        {
            Vector3 desiredPosition = playerTarget.position + offset;
            transform.position = new Vector3(desiredPosition.x, desiredPosition.y, -50);
        }
    }

    void LateUpdate()
    {
        Transform target = OverrideTarget != null ? OverrideTarget : playerTarget;

        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);

            // Handle zoom override
            if (cam != null)
            {
                float targetZoom = OverrideZoom > 0 ? OverrideZoom : defaultZoom;
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, smoothSpeed * Time.deltaTime);
            }
        }
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void TriggerShake(float duration = 0.1f, float magnitude = 0.1f)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private void OnValidate()
    {
        Vector3 pos = transform.position;

        if (playerTarget != null)
            pos = playerTarget.position + offset;

        transform.position = new Vector3(pos.x, pos.y, -10f);

        if (cam == null)
            cam = GetComponent<Camera>();

        if (cam != null)
        {
            defaultZoom = cam.orthographicSize;
        }
    }
}
