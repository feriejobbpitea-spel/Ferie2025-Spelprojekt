using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    public CrushingWalls crusher;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            crusher.ActivateWalls();
        }
    }
}