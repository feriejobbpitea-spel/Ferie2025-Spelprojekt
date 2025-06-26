[System.Serializable]
public class Item
{
    public string itemName;
    public int id;

    public Item(string name, int itemId)
    {
        itemName = name;
        id = itemId;
    }
}
