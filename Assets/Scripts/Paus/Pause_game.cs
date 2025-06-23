using UnityEngine;
using UnityEngine.UI;


public class Pause_game : MonoBehaviour
{

    public Menu menu; // Reference to the Menu component for the pause men
                     // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Update()
    {
        // Check for the Escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed, toggling pause.");
            Pause();
        }
    }


    // Update is called once per frame

    public void Pause()
    {
        int currentLives = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>().currentLives;
        Image pause = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>().pause;
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
            menu.HideMenu(); // Hide the pause menu
        }





    }
}
