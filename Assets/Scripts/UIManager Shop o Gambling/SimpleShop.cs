using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleShop : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public Sprite itemIcon;
        public int price;
    }

    [Header("Butiksdata")]
    public List<ShopItem> itemsForSale = new List<ShopItem>();

    [Header("Referenser")]
    public GameObject itemPrefab;
    public Transform itemContainer;
    public TMP_Text playerMoneyText;

    void Start()
    {
        BuildShop();
    }

    void BuildShop()
    {
        // Rensa tidigare objekt i shopen
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (var item in itemsForSale)
        {
            GameObject go = Instantiate(itemPrefab, itemContainer);

            // Hämta komponenterna direkt från prefab-roten
            go.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
            go.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = item.price + " coins";
            go.transform.Find("ItemHolder").Find("ItemIcon").GetComponent<Image>().sprite = item.itemIcon;

            // Köpknapp
            Button buyButton = go.transform.Find("BuyButton")?.GetComponent<Button>();
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(() => BuyItem(item));
            }
            else
            {
                Debug.LogError("BuyButton saknas i prefab!");
            }
        }

        UpdateMoneyUI();
    }

    void BuyItem(ShopItem item)
    {
        if (MoneyHolder.Instance.money >= item.price)
        {
            MoneyHolder.Instance.money -= item.price;
            Debug.Log("Du köpte: " + item.itemName);
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("Inte tillräckligt med pengar!");
        }
    }

    void UpdateMoneyUI()
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"{MoneyHolder.Instance.money}";
        }
    }
}