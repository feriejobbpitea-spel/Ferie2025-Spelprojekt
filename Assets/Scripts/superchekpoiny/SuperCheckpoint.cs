using UnityEngine;

public class SuperCheckpoint : MonoBehaviour
{
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            PlayerRespawn.Instance.SetSuperCheckpoint(transform.position);
            activated = true;
            Debug.Log("SuperCheckpoint aktiverad!");

            // (Valfritt) lägg till visuell feedback
            // t.ex. GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
}
