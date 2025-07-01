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

    public TMP_Text feedbackText;

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
                if (HasItemAlready(item.itemName) || (IsWeapon(item.itemName) && !InventoryManager.Instance.HasInventorySpaceForWeapon()))
                {
                    buyButton.interactable = false;
                }
                else
                {
                    buyButton.interactable = true;
                    buyButton.onClick.AddListener(() => BuyItem(item));
                }
            }
            else
            {
                Debug.LogError("BuyButton saknas i prefab!");
            }
        }

        UpdateMoneyUI();
        ClearFeedback();
    }

    void BuyItem(ShopItem item)
    {
        if (PlayerMoney.Instance.money < item.price)
        {
            ShowFeedback("Inte tillräckligt med pengar!");
            return;
        }

        // Kontrollera om item är vapen
        bool isWeapon = IsWeapon(item.itemName);

        // Kontrollera om spelaren redan har item (vapen eller annat)
        if (HasItemAlready(item.itemName))
        {
            ShowFeedback("Du har redan denna!");
            return;
        }

        // Om det är ett vapen, kolla om det finns plats i inventory
        if (isWeapon && !InventoryManager.Instance.HasInventorySpaceForWeapon())
        {
            ShowFeedback("Ingen plats i inventory för fler vapen!");
            return;
        }

        // Allt OK, dra pengar
        PlayerMoney.Instance.money -= item.price;
        UpdateMoneyUI();

        GameObject player = GameObject.Find("Player");

        // Ge spelaren item / vapen / effekt
        switch (item.itemName)
        {
            case "Konfetti":
                InventoryManager.Instance.AddConfettiGun();
                break;
            case "RayGun":
                InventoryManager.Instance.AddRayGun();
                break;
            case "Slangbella":
                InventoryManager.Instance.AddSlingshot();
                break;
            case "EmpVapen":
                InventoryManager.Instance.AddEmpGun();
                break;
            case "DoubleJump":
                if (player != null)
                    player.GetComponent<Movement>().doubleJump = true;
                break;
            case "SuperJump":
                if (player != null)
                    player.GetComponent<Movement>().bigJump = true;
                break;
            case "Time slow":
                if (player != null)
                    player.GetComponent<Movement>().timeSlow = true;
                break;
            case "Speed":
                if (player != null)
                    player.GetComponent<Movement>().superSpeed = 2;
                break;
            case "Hjärta":
                if (player != null)
                    player.GetComponent<PlayerHealthV2>().AddLife();
                break;
            default:
                Debug.LogWarning("Okänt föremål: " + item.itemName);
                break;
        }

        ShowFeedback("Du köpte: " + item.itemName);

        // Uppdatera UI för att t.ex. gråa ut knappar igen
        BuildShop();
    }

    // NY METOD: Kolla om item är ett vapen
    bool IsWeapon(string itemName)
    {
        switch (itemName)
        {
            case "Konfetti":
            case "RayGun":
            case "Slangbella":
            case "EmpVapen":
                return true;
            default:
                return false;
        }
    }

    bool HasItemAlready(string itemName)
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("Player hittades inte!");
            return false;
        }

        var movement = player.GetComponent<Movement>();

        switch (itemName)
        {
            case "Konfetti":
                return InventoryManager.Instance.HasConfettiGun();
            case "RayGun":
                return InventoryManager.Instance.HasRayGun();
            case "Slangbella":
                return InventoryManager.Instance.HasSlingshot();
            case "EmpVapen":
                return InventoryManager.Instance.HasEmpGun();
            case "DoubleJump":
                return movement != null && movement.doubleJump;
            case "SuperJump":
                return movement != null && movement.bigJump;
            case "Time slow":
                return movement != null && movement.timeSlow;
            case "Speed":
                return movement != null && movement.superSpeed > 1;
            case "Hjärta":
                PlayerHealthV2 health = player.GetComponent<PlayerHealthV2>();
                if (health != null)
                {
                    // Här låser vi köpet till max 4 hjärtan (maxLives)
                    return health.maxLives >= 4;
                }
                return false;
            default:
                return false;
        }
    }


    void UpdateMoneyUI()
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"{PlayerMoney.Instance.money}";
        }
    }

    void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 2f);
        }
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
