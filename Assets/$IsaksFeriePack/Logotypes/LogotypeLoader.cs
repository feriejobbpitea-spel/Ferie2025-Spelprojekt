using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogotypeLoader : MonoBehaviour
{
    [SerializeField] private float WaitTime = 5.0F;
    [SerializeField] private string SceneToLoad;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(WaitTime);
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneToLoad);
    }
}
