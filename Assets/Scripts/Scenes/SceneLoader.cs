using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : Singleton<SceneLoader> 
{
    public void LoadScene(string sceneName) 
    {
        StopAllCoroutines();
        StartCoroutine(LoadYourAsyncScene(sceneName));
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Time.timeScale = 1f; // Ensure time scale is reset after loading a new scene
    }
}
