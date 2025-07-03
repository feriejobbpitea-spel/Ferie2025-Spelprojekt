using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

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

            SetLocalizedText(go.transform.Find("ItemName")?.GetComponent<LocalizeStringEvent>(), "shop.item." + item.internalID + ".name");
            SetLocalizedText(go.transform.Find("ItemPrice")?.GetComponent<LocalizeStringEvent>(), "shop.item." + item.internalID + ".price");

            go.transform.Find("ItemHolder").Find("ItemIcon").GetComponent<Image>().sprite = item.itemSprite;

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

    void SetLocalizedText(LocalizeStringEvent localizeEvent, string key)
    {
        if (localizeEvent != null)
        {
            localizeEvent.StringReference.TableEntryReference = key;
        }
    }

    void BuyItem(ShopItem_SO item)
    {
        if (PlayerMoney.Instance.money < item.itemCost)
        {
            ShowLocalizedFeedback("shop.insufficient.money");
            return;
        }

        if (HasItemAlready(item.internalID))
        {
            ShowLocalizedFeedback("shop.already.own");
            return;
        }

        if (item.itemType == ItemType.Weapon && !InventoryManager.Instance.HasInventorySpaceForWeapon())
        {
            ShowLocalizedFeedback("shop.no.inventory.space");
            return;
        }

        PlayerMoney.Instance.money -= item.itemCost;
        UpdateMoneyUI(true); // Animated update

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

        ShowLocalizedFeedback("shop.purchase.success", item.ItemName.GetLocalizedString());
        BuildShop();
    }

    void ShowLocalizedFeedback(string key, string itemName = "")
    {
        if (feedbackText != null)
        {
            string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("ShopStrings", key).Replace("{item}", itemName);
            feedbackText.text = localizedText;

            feedbackText.DOKill();
            feedbackText.alpha = 0;
            feedbackText.DOFade(1f, 0.3f).SetEase(Ease.InOutQuad).SetUpdate(true);
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 2f);
        }
    }

    bool HasItemAlready(string internalID)
    {
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
                playerMoneyText.transform.DOKill();
                playerMoneyText.transform.localScale = Vector3.one * 1.2f;
                playerMoneyText.transform.DOScale(1f, 0.3f).SetUpdate(true).SetEase(Ease.OutBack);
            }
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