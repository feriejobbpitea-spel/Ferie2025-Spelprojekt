using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("Lower = moves slower (background). Higher = moves faster (foreground).")]
    [Range(0f, 1f)] public float parallaxFactor = 0.5f;

    private Vector3 previousCameraPosition;
    private Transform cameraTransform;
    private Vector3 startPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
        startPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - previousCameraPosition;
        Vector3 parallaxMovement = new Vector3(cameraDelta.x * parallaxFactor, cameraDelta.y * parallaxFactor, 0);
        transform.position += parallaxMovement;
        previousCameraPosition = cameraTransform.position;
    }
}
