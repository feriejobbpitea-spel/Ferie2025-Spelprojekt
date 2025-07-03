using UnityEngine;

public class OneWayLock2D : MonoBehaviour
{
    public string playerTag = "Player"; // Ange tagg för spelaren

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        if (col == null)
        {
            Debug.LogError("Ingen Collider2D hittades på objektet.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("Collidern bör vara en trigger från början. Sätter isTrigger = true.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Gör collidern solid så man inte kan gå tillbaka
            col.isTrigger = false;
        }
    }
}

