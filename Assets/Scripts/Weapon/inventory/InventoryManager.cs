using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : Singleton<InventoryManager>
{
    public Transform playerTransform;

    [Header("UI Slots")]
    public List<Image> slots;


    [Header("Prefabs")]
    public GameObject meleePrefab;
    public Sprite meleeIcon;

    public GameObject empPrefab;
    public Sprite empIcon;

    public GameObject rayGunPrefab;
    public Sprite rayGunIcon;

    public GameObject slingshotPrefab;
    public Sprite slingshotIcon;

    public GameObject confettiGunPrefab;        // 🆕 Confetti gun prefab
    public Sprite confettiGunIcon;              // 🆕 Confetti gun icon

    private GameObject currentWeapon;
    private GameObject activeBeam;

    private GameObject[] inventoryWeapons = new GameObject[4];
    private Sprite[] inventoryIcons = new Sprite[4];

    void Start()
    {
        inventoryWeapons[0] = meleePrefab;
        inventoryIcons[0] = meleeIcon;

        EquipWeapon(0); // Equip melee at start
        UpdateInventoryUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) EquipWeapon(3);

       
    }

    void EquipWeapon(int index)
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
        if (slots == null)
            return;

        for (int i = 0; i < slots.Count; i++)
        {
            if (inventoryIcons[i] != null)
            {
                slots[i].sprite = inventoryIcons[i];
                slots[i].color = Color.white;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0);
            }
        }
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

    public void AddConfettiGun() // 🆕
    {
        AddWeaponToNextSlot(confettiGunPrefab, confettiGunIcon);
    }

    private void AddWeaponToNextSlot(GameObject weaponPrefab, Sprite icon)
    {
        if (weaponPrefab == null)
            return;

        // Kontrollera om vapnet redan finns
        for (int i = 0; i < inventoryWeapons.Length; i++)
        {
            Debug.Log($"Checking slot {i}: {inventoryWeapons[i]} against {weaponPrefab}");
            if (inventoryWeapons[i] == weaponPrefab)
            {
                Debug.Log("Du har redan detta vapen.");
                return;
            }
        }

        // Sök efter nästa lediga slot (förutom slot 0)
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

        Debug.Log("Inventory fullt – kunde inte lägga till nytt vapen.");
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

        // ❌ Ta INTE bort vapen från inventory – bara återgå till melee
        EquipWeapon(0); // Växla till melee
    }

}
