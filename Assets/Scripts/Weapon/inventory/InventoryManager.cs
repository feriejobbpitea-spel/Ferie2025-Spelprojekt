using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public Transform playerTransform;

    [Header("UI Slots")]
    public List<Image> slots;

    [Header("Highlight Objects")]
    public List<GameObject> slotHighlights;

    [Header("Prefabs")]
    public GameObject meleePrefab;
    public Sprite meleeIcon;

    public GameObject empPrefab;
    public Sprite empIcon;

    public GameObject rayGunPrefab;
    public Sprite rayGunIcon;

    public GameObject slingshotPrefab;
    public Sprite slingshotIcon;

    public GameObject confettiGunPrefab;
    public Sprite confettiGunIcon;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip weaponSwitchClip;

    public GameObject currentWeapon;
    private GameObject activeBeam;

    private GameObject[] inventoryWeapons = new GameObject[4];
    private Sprite[] inventoryIcons = new Sprite[4];

    private int activeSlotIndex = 0;

    private List<PickupItem> collectedItems = new List<PickupItem>();

    void Start()
    {
        inventoryWeapons[0] = meleePrefab;
        inventoryIcons[0] = meleeIcon;
        WeaponSlot(0);
        UpdateInventoryUI();
    }

    public List<PickupItem> GetCollectedItems()
    {
        return collectedItems;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            AddConfettiGun();
            AddEmpGun();
            AddRayGun();
            AddSlingshot();
        }

        if (Input.GetKeyDown(GetBoundKey("WeaponSlot1"))) WeaponSlot(0);
        else if (Input.GetKeyDown(GetBoundKey("WeaponSlot2"))) WeaponSlot(1);
        else if (Input.GetKeyDown(GetBoundKey("WeaponSlot3"))) WeaponSlot(2);
        else if (Input.GetKeyDown(GetBoundKey("WeaponSlot4"))) WeaponSlot(3);
    }

    KeyCode GetBoundKey(string action)
    {
        string keyStr = PlayerPrefs.GetString("bind_" + action, "");
        if (Enum.TryParse<KeyCode>(keyStr, out KeyCode key))
        {
            return key;
        }

        switch (action)
        {
            case "WeaponSlot1": return KeyCode.Alpha1;
            case "WeaponSlot2": return KeyCode.Alpha2;
            case "WeaponSlot3": return KeyCode.Alpha3;
            case "WeaponSlot4": return KeyCode.Alpha4;
            default: return KeyCode.None;
        }
    }

    public void WeaponSlot(int index)
    {
        if (index < 0 || index >= inventoryWeapons.Length) return;
        if (inventoryWeapons[index] == null)
        {
            Debug.LogWarning($"Inget vapen i slot {index + 1}");
            return;
        }

        if (activeBeam != null)
        {
            Destroy(activeBeam);
            activeBeam = null;
        }

        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(inventoryWeapons[index], playerTransform);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        activeSlotIndex = index;

        // Spela upp ljud vid byte
        if (audioSource != null && weaponSwitchClip != null)
        {
            audioSource.PlayOneShot(weaponSwitchClip);
        }

        UpdateInventoryUI();
    }

    public GameObject[] GetInventoryWeapons()
    {
        return inventoryWeapons;
    }

    public void DestroyCurrentWeapon()
    {
        if (activeBeam != null)
        {
            Destroy(activeBeam);
            activeBeam = null;
        }

        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }
    }

    public int GetWeaponIndex(GameObject prefab)
    {
        for (int i = 0; i < inventoryWeapons.Length; i++)
        {
            if (inventoryWeapons[i] == prefab)
                return i;
        }
        return -1;
    }

    void UpdateInventoryUI()
    {
        if (slots == null || slotHighlights == null) return;

        for (int i = 0; i < slotHighlights.Count; i++)
        {
            slotHighlights[i].transform.DOKill();
            slotHighlights[i].SetActive(false);
            slotHighlights[i].transform.localScale = Vector3.one;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            GameObject parentSlot = slots[i].transform.parent.gameObject;

            if (inventoryWeapons[i] != null && inventoryIcons[i] != null)
            {
                Sprite newIcon = inventoryIcons[i];
                Sprite currentIcon = slots[i].sprite;

                parentSlot.SetActive(true);

                if (currentIcon != newIcon)
                {
                    slots[i].sprite = newIcon;
                    slots[i].color = new Color(1, 1, 1, 0);
                    slots[i].DOFade(1f, 0.3f).SetEase(Ease.InOutQuad);
                }
                else
                {
                    slots[i].color = Color.white;
                }

                if (i == activeSlotIndex && i < slotHighlights.Count)
                {
                    slotHighlights[i].SetActive(true);
                    slotHighlights[i].transform.localScale = Vector3.zero;
                    slotHighlights[i].transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);
                }
            }
            else
            {
                if (slots[i].color.a > 0)
                {
                    slots[i].DOFade(0f, 0.3f).OnComplete(() =>
                    {
                        slots[i].sprite = null;
                        parentSlot.SetActive(false);
                    });
                }
                else
                {
                    slots[i].sprite = null;
                    parentSlot.SetActive(false);
                }
            }
        }
    }

    // ----- Hantera plockade items -----

    public void AddItem(PickupItem item)
    {
        if (item == null) return;

        if (!collectedItems.Contains(item))
        {
            collectedItems.Add(item);
            Debug.Log($"Lagt till item i inventory: {item.itemName}");
        }
        else
        {
            Debug.Log($"Item {item.itemName} finns redan i inventory.");
        }
    }

    public bool GiveItemToShop(string itemName)
    {
        PickupItem item = collectedItems.Find(i => i.itemName == itemName);
        if (item != null)
        {
            collectedItems.Remove(item);
            Debug.Log($"Gav {itemName} till butiksägaren.");
            return true;
        }
        Debug.Log($"Item {itemName} finns inte i inventory.");
        return false;
    }

    public void AddEmpGun()
    {
        AddWeaponToNextSlot(empPrefab, empIcon);
    }

    public void AddRayGun()
    {
        AddWeaponToNextSlot(rayGunPrefab, rayGunIcon);
    }

    public void AddSlingshot()
    {
        AddWeaponToNextSlot(slingshotPrefab, slingshotIcon);
    }

    public void AddConfettiGun()
    {
        AddWeaponToNextSlot(confettiGunPrefab, confettiGunIcon);
    }

    public bool HasEmpGun()
    {
        return HasWeapon(empPrefab);
    }

    public bool HasRayGun()
    {
        return HasWeapon(rayGunPrefab);
    }

    public bool HasSlingshot()
    {
        return HasWeapon(slingshotPrefab);
    }

    public bool HasConfettiGun()
    {
        return HasWeapon(confettiGunPrefab);
    }

    private bool HasWeapon(GameObject weaponPrefab)
    {
        foreach (var w in inventoryWeapons)
        {
            if (w == weaponPrefab)
                return true;
        }
        return false;
    }

    public bool HasInventorySpaceForWeapon()
    {
        for (int i = 1; i < inventoryWeapons.Length; i++) // Slot 0 är melee
        {
            if (inventoryWeapons[i] == null)
                return true;
        }
        return false;
    }

    private void AddWeaponToNextSlot(GameObject weaponPrefab, Sprite icon)
    {
        if (weaponPrefab == null)
            return;

        if (!HasInventorySpaceForWeapon())
        {
            Debug.Log("Inventory fullt – kunde inte lägga till nytt vapen.");
            return;
        }

        if (HasWeapon(weaponPrefab))
        {
            Debug.Log("Du har redan detta vapen.");
            return;
        }

        for (int i = 1; i < inventoryWeapons.Length; i++)
        {
            if (inventoryWeapons[i] == null)
            {
                inventoryWeapons[i] = weaponPrefab;
                inventoryIcons[i] = icon;
                UpdateInventoryUI();
                return;
            }
        }
    }

    public void DropWeaponOnDeath()
    {
        if (activeBeam != null)
        {
            Destroy(activeBeam);
            activeBeam = null;
        }

        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
            currentWeapon = null;
        }

        WeaponSlot(0); // Återgå till melee
    }
}

