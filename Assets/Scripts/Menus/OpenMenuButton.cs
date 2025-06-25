using System;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenuButton : MonoBehaviour
{
    public Menu Menu;
    private Button _button;
    public bool CanOpenAndClose;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError("OpenMenuButton requires a Button component.");
            return;
        }
        
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ClickedButton);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ClickedButton);
    }

    private void ClickedButton()
    {
        if (CanOpenAndClose)
        {
            if(Menu.IsOpen)
                Menu.HideMenu();
            else
                MenuManager.Instance.OpenMenu(Menu);
        }
        else
        {
            MenuManager.Instance.OpenMenu(Menu);
        }
    }
}
