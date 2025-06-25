using System;
using UnityEngine;

[Serializable]
public class Shop_HUDHandler
{
    public GameObject ShopHUD;

    public void OpenShopHUD()
    {
        if (ShopHUD != null)
        {
            ShopHUD.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ShopHUD is not assigned in the inspector.");
        }
    }

    public void CloseShopHUD()
    {
        if (ShopHUD != null)
        {
            ShopHUD.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ShopHUD is not assigned in the inspector.");
        }
    }
}
