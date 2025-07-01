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
    public List<GameObject> slotHighlights;  // Dra in highlight-objekten i Unity

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

    public GameObject currentWeapon;
    private GameObject activeBeam;

    private GameObject[] inventoryWeapons = new GameObject[4];
    private Sprite[] inventoryIcons = new Sprite[4];

    private int activeSlotIndex = 0;  // Håll koll på vilken slot som är aktiv

    // Ny lista för plockade items (t.ex bilder, andra objekt)
    private List<PickupItem> collectedItems = new List<PickupItem>();

    void Start()
    {
        inventoryWeapons[0] = meleePrefab;
        inventoryIcons[0] = meleeIcon;

        UpdateInventoryUI();
    }

    void Update()
    {
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

    void WeaponSlot(int index)
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

        activeSlotIndex = index;  // Uppdatera aktiv slot

        UpdateInventoryUI();
    }

    public void SetActiveBeam(GameObject beam)
    {
        if (activeBeam != null)
        {
            Destroy(activeBeam);
        }
        activeBeam = beam;
    }

    void UpdateInventoryUI()
    {
        if (slots == null || slotHighlights == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            GameObject parentSlot = slots[i].transform.parent.gameObject;

            if (inventoryIcons[i] != null)
            {
                slots[i].sprite = inventoryIcons[i];
                slots[i].color = Color.white;
                parentSlot.SetActive(true);

                if (i < slotHighlights.Count)
                    slotHighlights[i].SetActive(i == activeSlotIndex);
            }
            else
            {
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0);
                parentSlot.SetActive(false);

                if (i < slotHighlights.Count)
                    slotHighlights[i].SetActive(false);
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

    // ----- Befintliga metoder för vapen -----

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

    // ----- NYA METODER för att kolla om spelaren redan har vapnet -----

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

    // Privat hjälpfunktion som kollar om weaponPrefab finns i inventoryWeapons-arrayen
    private bool HasWeapon(GameObject weaponPrefab)
    {
        foreach (var w in inventoryWeapons)
        {
            if (w == weaponPrefab)
                return true;
        }
        return false;
    }

    // NY metod som kollar om det finns plats i inventory (för vapen)
    public bool HasInventorySpaceForWeapon()
    {
        for (int i = 1; i < inventoryWeapons.Length; i++) // Hoppa över slot 0 som är melee
        {
            if (inventoryWeapons[i] == null)
                return true;
        }
        return false;
    }

    // ÄNDRA DENNA METOD (lägg till kontroll innan tillägg)
    private void AddWeaponToNextSlot(GameObject weaponPrefab, Sprite icon)
    {
        if (weaponPrefab == null)
            return;

        // Kolla om det finns plats först
        if (!HasInventorySpaceForWeapon())
        {
            Debug.Log("Inventory fullt – kunde inte lägga till nytt vapen.");
            return;
        }

        // Kolla om spelaren redan har vapnet
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

        // Ta INTE bort vapen från inventory – bara återgå till melee
        WeaponSlot(0); // Växla till melee
    }
}
