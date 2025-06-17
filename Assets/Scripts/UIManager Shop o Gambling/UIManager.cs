using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject storeUI;
    public GameObject gamblingUI;

    void Start()
    {
        ShowStore(); // Visa butiken när spelet startar
    }

    public void ShowStore()
    {
        storeUI.SetActive(true);
        gamblingUI.SetActive(false);
    }

    public void ShowGambling()
    {
        storeUI.SetActive(false);
        gamblingUI.SetActive(true);
    }
}
