using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<string> items = new List<string>();

    public void AddItem(string item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            Debug.Log($"Lagt till {item} i inventory.");
        }
    }

    public bool HasItem(string item)
    {
        return items.Contains(item);
    }

    public void RemoveItem(string item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"Tagit bort {item} från inventory.");
        }
    }
    public bool HasPicture()
    {
        return HasItem("Picture");
    }

    public void RemovePicture()
    {
        RemoveItem("Picture");
    }
}