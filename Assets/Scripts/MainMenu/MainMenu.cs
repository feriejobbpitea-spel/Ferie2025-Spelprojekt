using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string GameSceneName = "GameScene";

    public void QuitGame() 
    {
        Screen.fullScreen = false; 
    }

    public void StartGame() 
    {
        SceneLoader.Instance.LoadScene(GameSceneName);
    }
}
