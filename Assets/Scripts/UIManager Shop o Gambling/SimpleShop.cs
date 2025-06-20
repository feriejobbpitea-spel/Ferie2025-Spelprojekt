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
        // Rensa tidigare objekt i shopen
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (var item in itemsForSale)
        {
            GameObject go = Instantiate(itemPrefab, itemContainer);

            // H�mta komponenterna direkt fr�n prefab-roten
            go.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
            go.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = item.price + " kr";
            go.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.itemIcon;

            // K�pknapp
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
        if (playerMoney >= item.price)
        {
            playerMoney -= item.price;
            Debug.Log("Du k�pte: " + item.itemName);
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("Inte tillr�ckligt med pengar!");
        }
    }

    void UpdateMoneyUI()
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = "Pengar: " + playerMoney;
        }
    }
}