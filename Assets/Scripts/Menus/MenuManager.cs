using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    private Menu[] _allMenus;

    private void Awake()
    {
        _allMenus = FindObjectsByType<Menu>(FindObjectsSortMode.InstanceID);
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
