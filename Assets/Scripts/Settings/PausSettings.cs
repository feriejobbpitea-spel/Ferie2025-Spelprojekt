using UnityEngine;

public class PausSettings : MonoBehaviour
{
    public GameObject pauseMenu; // Assign this in the Inspector
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void TogglePause()
    {
         pauseMenu.SetActive(!pauseMenu.activeSelf);
        //Debug.Log("Game is " + (isPaused ? "paus is showing" : "paus is invisible"));

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
