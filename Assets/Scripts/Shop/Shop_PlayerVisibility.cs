using UnityEngine;

[System.Serializable]
public class Shop_PlayerVisibility
{

    private GameObject _player;
    public void ShowPlayer()
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
        _player.SetActive(true);
    }
    public void HidePlayer()
    {
        if(_player == null) {             
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
                return;
            }
        }
        _player.SetActive(false);
    }

}
