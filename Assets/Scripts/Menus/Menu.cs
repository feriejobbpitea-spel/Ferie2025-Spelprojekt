using UnityEngine;

public class Menu : MonoBehaviour
{
    public bool ShowOnStart = false;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Menu script requires a Canvas component on the same GameObject.");
        }
    }

    private void Start()
    {
        if (ShowOnStart)
            ShowMenu();
        else
            HideMenu();
    }

    public void ShowMenu() 
    {
        canvas.enabled = true;
    }

    public void HideMenu() 
    {
        canvas.enabled = false;
    }

}
