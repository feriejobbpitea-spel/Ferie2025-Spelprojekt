using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int maxItems = 20;

    // Lägg till item, returnerar true om lyckades, false om fullt
    public bool AddItem(Item item)
    {
        if (items.Count >= maxItems)
        {
            Debug.Log("Inventory är fullt!");
            return false;
        }
        items.Add(item);
        Debug.Log("Lade till " + item.itemName);
        return true;
    }

    // Ta bort item
    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log("Tog bort " + item.itemName);
        }
        else
        {
            Debug.Log("Item finns inte i inventory!");
        }
    }

    // Kolla om item finns
    public bool HasItem(Item item)
    {
        return items.Contains(item);
    }
}
