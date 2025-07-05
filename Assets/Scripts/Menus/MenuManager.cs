using UnityEngine;
using System.Collections.Generic;   

public class MenuManager : Singleton<MenuManager>
{
    private List<Menu> _allMenus = new();

    public void InitializeMenu(Menu menu) => _allMenus.Add(menu); 

    public void CloseAllMenus() 
    {
        foreach (var m in _allMenus)
        {
            m.HideMenu();
        }
    }

    public void OpenMenu(Menu menu) 
    {
        foreach (var m in _allMenus)
        {
            if (m != menu)
            {
                m.HideMenu();
            }
        }
        menu.ShowMenu();
    }
}
