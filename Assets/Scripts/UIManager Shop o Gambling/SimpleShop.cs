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
        if (PlayerMoney.Instance.money >= item.price)
        {
            GameObject player = GameObject.Find("Player");
            PlayerMoney.Instance.money -= item.price;
            Debug.Log("Du köpte: " + item.itemName);
            UpdateMoneyUI();
            switch (item.itemName)
            {
                case "Konfetti":
                    Debug.Log("´du fick konfetti!");
                    InventoryManager.Instance.AddConfettiGun();
                    break;
                case "RayGun":
                    InventoryManager.Instance.AddRayGun();
                    break;
                case "Slangebella":
                    InventoryManager.Instance.AddSlingshot();
                    break;
                case "EmpVapen":
                    InventoryManager.Instance.AddEmpGun();
                    break;
                case "DoubleJump":
                    player.GetComponent<Movement>().doubleJump = true;
                    break;
                case "SuperJump":
                    player.GetComponent<Movement>().bigJump = true;
                    break;
                case "Time slow":
                    player.GetComponent<Movement>().timeSlow = true;
                    break;
                case "Speed":
                    player.GetComponent<Movement>().superSpeed = 2;
                    break;
                default:
                    Debug.LogWarning("Okänt föremål: " + item.itemName);
                    break;
            }
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
            playerMoneyText.text = $"{PlayerMoney.Instance.money}";
        }
    }
}