using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("Lower = moves slower (background). 1 = matches player exactly.")]
    [Range(0f, 1f)] public float parallaxFactor = 0.5f;

    public Transform player; // Assign this via Inspector or dynamically

    private Vector3 startLocalPosition;
    private Vector3 playerStartPosition;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("ParallaxLayer: Player not assigned!");
            return;
        }

        startLocalPosition = transform.localPosition;
        playerStartPosition = player.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 playerOffset = player.position - playerStartPosition;
        Vector3 parallaxOffset = new Vector3(playerOffset.x * parallaxFactor, playerOffset.y * parallaxFactor, 0);
        transform.localPosition = startLocalPosition + parallaxOffset;
    }
}
