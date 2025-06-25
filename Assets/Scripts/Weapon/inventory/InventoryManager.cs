using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] weaponPrefabs;
    public Sprite[] weaponIcons;

    public Image slot1;
    public Image slot2;
    public Image slot3;
    public Image slot4;

    public Transform playerTransform;

    private GameObject currentWeapon;
    private GameObject activeBeam;

    void Start()
    {
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
        if (index < 0 || index >= weaponPrefabs.Length) return;
        if (weaponPrefabs[index] == null)
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

        currentWeapon = Instantiate(weaponPrefabs[index], playerTransform);
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
        Image[] slots = { slot1, slot2, slot3, slot4 };

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < weaponIcons.Length && weaponIcons[i] != null)
            {
                slots[i].sprite = weaponIcons[i];
                slots[i].color = Color.white;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0);
            }
        }
    }
}
