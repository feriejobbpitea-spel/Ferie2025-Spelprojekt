using UnityEngine;

[System.Serializable]
public class Shop_PlayerVisibility
{
    private GameObject _player;
    private GameObject _gfxChild;

    private void FindPlayerAndGFX()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
                return;
            }
        }

        if (_gfxChild == null && _player != null)
        {
            Transform gfxTransform = _player.transform.Find("GFX");
            if (gfxTransform != null)
            {
                _gfxChild = gfxTransform.gameObject;
            }
            else
            {
                Debug.LogError("GFX child not found under Player!");
            }
        }
    }

    public void ShowPlayer()
    {
        FindPlayerAndGFX();

        if (_gfxChild != null)
        {
            _gfxChild.SetActive(true);
        }
    }

    public void HidePlayer()
    {
        FindPlayerAndGFX();

        if (_gfxChild != null)
        {
            _gfxChild.SetActive(false);
        }
    }
}
