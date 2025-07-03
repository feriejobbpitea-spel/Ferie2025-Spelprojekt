using UnityEngine;
using UnityEngine.UI;


public class Pause_game : MonoBehaviour
{
    public Canvas PauseMenuCanvas;
    public Menu PauseMenu;
    public Canvas settingsUI;
    private void Update()
    {
        // Check for the Escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape)  ) 
        {
            if (!settingsUI.enabled)
            {
                Debug.Log("Escape key pressed, toggling pause.");
                Pause();
            }
            else { settingsUI.enabled = false; PauseMenuCanvas.enabled = true; }
            
        }
        
    }
    
    public void ShowPauseCanvas() { PauseMenuCanvas.enabled = true; }
    // Update is called once per frame

    public void Pause()
    {
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

        PauseMenu.ShowMenu();

       


    }
}
