using UnityEngine;
using UnityEngine.UI;


public class Pause_game : MonoBehaviour
{
    public Menu PauseMenu;
    public Menu SettingsMenu;
    private void Update()
    {
        // Check for the Escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (!SettingsMenu.IsOpen && !PauseMenu.IsOpen)
            {
                Pause();
            }
            else 
            {
                Unpause();
            }            
        }        
    }
    
    
    public void Unpause() 
    {
        Time.timeScale = 1;
        MenuManager.Instance.CloseAllMenus();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        MenuManager.Instance.OpenMenu(PauseMenu);
        /*
        int currentLives = GameObject.FindWithTag("Player").GetComponent<PlayerHealthV2>().currentLives;
        Image pause = GameObject.FindWithTag("Player").GetComponent<PlayerHealthV2>().pause;
        // Toggle the pause state of the game
        if (Time.timeScale != 0f && currentLives > 0)
        {
            pause.gameObject.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
        else if (currentLives > 0)
        {
            pause.gameObject.SetActive(false);
            Time.timeScale = 1f; // Resume the game
        }
        PauseMenu.ShowMenu();*/
    }
}
