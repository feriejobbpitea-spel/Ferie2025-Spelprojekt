using UnityEngine;
using UnityEngine.Localization;

public enum ItemType { Weapon, Upgrade, Consumable }

[CreateAssetMenu]
public class ShopItem_SO : ScriptableObject
{ 
    public LocalizedString itemName; // Lägg detta i Unity Editor
    public Sprite itemSprite;
    public int itemCost;
    public ItemType itemType;
    public string internalID;

    public LocalizedString ItemName => itemName;
}