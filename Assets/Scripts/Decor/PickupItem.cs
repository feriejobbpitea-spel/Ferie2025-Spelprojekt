using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;

    private bool isPickedUp = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPickedUp) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Plockade upp {itemName}!");
            InventoryManager.Instance.AddItem(this);  // AddItem tar emot SecretItem nu
            isPickedUp = true;
            gameObject.SetActive(false);
        }
    }
}
