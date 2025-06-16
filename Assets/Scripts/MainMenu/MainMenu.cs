using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public string GameSceneName = "GameScene";

    public void QuitGame() 
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the editor
#endif
    }

    public void StartGame() 
    {
        SceneLoader.Instance.LoadScene(GameSceneName);
    }
}
