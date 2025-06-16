using System;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenuButton : MonoBehaviour
{
    public Menu Menu;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError("OpenMenuButton requires a Button component.");
            return;
        }
        
        _button.onClick.AddListener(OpenMenu);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OpenMenu);
    }

    private void OpenMenu()
    {
        MenuManager.Instance.OpenMenu(Menu);
    }
}
