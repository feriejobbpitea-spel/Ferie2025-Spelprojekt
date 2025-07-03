using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleShop : Singleton<SimpleShop>
{
    [Header("Butiksdata")]
    public List<ShopItem_SO> itemsForSale = new List<ShopItem_SO>();

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
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (var item in itemsForSale)
        {
            GameObject go = Instantiate(itemPrefab, itemContainer);

            go.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.ItemName;
            go.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = item.ItemCost + " datachips";
            go.transform.Find("ItemHolder").Find("ItemIcon").GetComponent<Image>().sprite = item.ItemSprite;

            Button buyButton = go.transform.Find("BuyButton")?.GetComponent<Button>();
            if (buyButton != null)
            {
                if ((HasItemAlready(item.internalID)) || (item.itemType == ItemType.Weapon && !InventoryManager.Instance.HasInventorySpaceForWeapon()))
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

    void BuyItem(ShopItem_SO item)
    {
        if (PlayerMoney.Instance.money < item.ItemCost)
        {
            ShowFeedback("Inte tillräckligt med pengar!");
            return;
        }

        if (HasItemAlready(item.internalID))
        {
            ShowFeedback("Du har redan denna!");
            return;
        }

        if (item.itemType == ItemType.Weapon && !InventoryManager.Instance.HasInventorySpaceForWeapon())
        {
            ShowFeedback("Ingen plats i inventory för fler vapen!");
            return;
        }

        PlayerMoney.Instance.money -= item.ItemCost;
        UpdateMoneyUI();


        switch (item.internalID)
        {
            case "Confetti":
                InventoryManager.Instance.AddConfettiGun(); break;
            case "RayGun":
                InventoryManager.Instance.AddRayGun(); break;
            case "Slingshot":
                InventoryManager.Instance.AddSlingshot(); break;
            case "EmpGun":
                InventoryManager.Instance.AddEmpGun(); break;
            case "DoubleJump":
                Movement.Instance.doubleJump = true; break;
            case "SuperJump":
                Movement.Instance.bigJump = true; break;
            case "TimeSlow":
                Movement.Instance.timeSlow = true; break;
            case "Speed":
                Movement.Instance.superSpeed = 2; break;
            case "Heart":
                PlayerHealthV2.Instance.AddLife(); break;
            default:
                Debug.LogWarning("Okänt föremål: " + item.internalID); break;
        }

        ShowFeedback("Du köpte: " + item.ItemName);
        BuildShop();
    }

    bool HasItemAlready(string internalID)
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("Player hittades inte!");
            return false;
        }

        var movement = Movement.Instance;
        var health = PlayerHealthV2.Instance;

        switch (internalID)
        {
            case "Confetti":
                return InventoryManager.Instance.HasConfettiGun();
            case "RayGun":
                return InventoryManager.Instance.HasRayGun();
            case "Slingshot":
                return InventoryManager.Instance.HasSlingshot();
            case "EmpGun":
                return InventoryManager.Instance.HasEmpGun();
            case "DoubleJump":
                return movement != null && movement.doubleJump;
            case "SuperJump":
                return movement != null && movement.bigJump;
            case "TimeSlow":
                return movement != null && movement.timeSlow;
            case "Speed":
                return movement != null && movement.superSpeed > 1;
            case "Heart":
                return health != null && health.currentLives >= health.maxLives;
            default:
                return false;
        }
    }



    public void UpdateMoneyUI(bool animated = false)
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = $"{PlayerMoney.Instance.money}";

            if (animated)
            {
                // Animate money text pop
                playerMoneyText.transform.DOKill(); // Stop previous tweens
                playerMoneyText.transform.localScale = Vector3.one * 1.2f;
                playerMoneyText.transform.DOScale(1f, 0.3f).SetUpdate(true).SetEase(Ease.OutBack);
            }
        }
    }

    void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.DOKill();
            feedbackText.alpha = 0;
            feedbackText.DOFade(1f, 0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 2f);
        }
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.DOFade(0f, 0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
        }
    }
}
