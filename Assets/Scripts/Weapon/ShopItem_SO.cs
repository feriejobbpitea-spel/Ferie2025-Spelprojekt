using UnityEngine;

public enum ItemType { Weapon, Upgrade, Consumable }

[CreateAssetMenu]
public class ShopItem_SO : ScriptableObject
{
    public string ItemName;
    public Sprite ItemSprite;
    public int ItemCost;

    public ItemType itemType;
    public string internalID;
}
