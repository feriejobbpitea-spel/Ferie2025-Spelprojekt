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
    public int playerMoney = 1000;

    void Start()
    {
        BuildShop();
    }

    void BuildShop()
    {
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (var item in itemsForSale)
        {
            GameObject go = Instantiate(itemPrefab, itemContainer);
            go.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemName;
            go.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = item.price + " kr";
            go.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.itemIcon;

            go.transform.Find("BuyButton")
                .GetComponent<Button>().onClick
                .AddListener(() => BuyItem(item));
        }

        UpdateMoneyUI();
    }

    void BuyItem(ShopItem item)
    {
        if (playerMoney >= item.price)
        {
            playerMoney -= item.price;
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
        playerMoneyText.text = "Pengar: " + playerMoney;
    }
}
