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
        if(!IsCloseToShop())
            return;

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (_inShop) {
                 // Resume time when exiting shop
                ExitShop();
            }

            else
            {
                 // Pause time when entering shop
                EnterShop();
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
    }

    public void ExitShop() 
    {
        Time.timeScale = 1f;
        PlayerHandler.ShowPlayer();
        _inShop = false;
        CameraHandler.MoveCameraAwayFromShop();
        ShopHUDHandler.CloseShopHUD();
        DialogueHandler.PlayExitShopDialogue();
    }


}
