using UnityEngine;

public class Shop : MonoBehaviour
{
    public float InteractDistance = 3;

    public Shop_CameraHandler CameraHandler;
    public Shop_HUDHandler ShopHUDHandler;
    public Shop_PlayerVisibility PlayerHandler;
    public Shop_DialogueHandler DialogueHandler;

    private bool _inShop = false;
    private Transform _player;

    // Sparar vilket vapen som var aktivt innan shoppen öppnades
    private GameObject _savedWeaponPrefab;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (!IsCloseToShop())
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_inShop)
            {
                ExitShop();
            }
            else
            {
                EnterShop();
                TryGiveItemToShop();
                SimpleShop.Instance.UpdateMoneyUI();
            }
        }
    }

    private void Start()
    {
        ShopHUDHandler.CloseShopHUD();
        DialogueHandler.Initialize();
    }

    public bool IsCloseToShop() => Vector2.Distance(_player.position, transform.position) < InteractDistance;

    private void EnterShop()
    {
        Time.timeScale = 0f;
        PlayerHandler.HidePlayer();
        _inShop = true;
        CameraHandler.MoveCameraToShop();
        ShopHUDHandler.OpenShopHUD();
        DialogueHandler.PlayEnterShopDialogue();

        // Despawna vapnet och spara vilket det var
        SaveAndDespawnWeapon();
    }

    public void ExitShop()
    {
        Time.timeScale = 1f;
        PlayerHandler.ShowPlayer();
        _inShop = false;
        CameraHandler.MoveCameraAwayFromShop();
        ShopHUDHandler.CloseShopHUD();
        DialogueHandler.PlayExitShopDialogue();

        PlayerMoney.Instance.UpdateMoneyUI();

        // Spawna tillbaka vapnet när du lämnar shoppen
        RespawnSavedWeapon();
    }

    private void TryGiveItemToShop()
    {
        var collectedItems = InventoryManager.Instance.GetCollectedItems();

        if (collectedItems.Count == 0)
        {
            Debug.Log("Inga items att ge till shoppen.");
            return;
        }

        var itemToGive = collectedItems[collectedItems.Count - 1];

        if (InventoryManager.Instance.GiveItemToShop(itemToGive.itemName))
        {
            Debug.Log($"Gav {itemToGive.itemName} till shoppen.");
            InventoryManager.Instance.AddConfettiGun();
        }
        else
        {
            Debug.Log($"Kunde inte ge {itemToGive.itemName} till shoppen.");
        }
    }

    private void SaveAndDespawnWeapon()
    {
        var currentWeapon = InventoryManager.Instance.currentWeapon;
        if (currentWeapon == null)
        {
            _savedWeaponPrefab = null;
            return;
        }

        GameObject[] inventoryWeapons = InventoryManager.Instance.GetInventoryWeapons();

        _savedWeaponPrefab = null;

        foreach (var prefab in inventoryWeapons)
        {
            if (prefab == null) continue;

            // Jämför namn (kan justeras om du har unika ID:n)
            if (currentWeapon.name.Contains(prefab.name))
            {
                _savedWeaponPrefab = prefab;
                break;
            }
        }

        // Despawna vapnet
        InventoryManager.Instance.DestroyCurrentWeapon();
    }

    private void RespawnSavedWeapon()
    {
        if (_savedWeaponPrefab == null)
        {
            // Om inget sparades, spawna melee (default)
            InventoryManager.Instance.WeaponSlot(0);
            return;
        }

        int index = InventoryManager.Instance.GetWeaponIndex(_savedWeaponPrefab);

        if (index >= 0)
        {
            InventoryManager.Instance.WeaponSlot(index);
        }
        else
        {
            InventoryManager.Instance.WeaponSlot(0);
        }

        _savedWeaponPrefab = null;
    }
}
