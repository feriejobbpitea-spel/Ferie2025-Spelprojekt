using UnityEngine;

public class FlexibleWallTrigger : MonoBehaviour
{
    [Header("Väggar med CrushingWalls")]
    public CrushingWalls crusher;

    [Header("Portar med PortCloser")]
    public PortCloser portCloser;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (crusher != null)
        {
            crusher.ActivateWalls();
        }

        if (portCloser != null)
        {
            portCloser.ClosePort();
        }
    }
}

