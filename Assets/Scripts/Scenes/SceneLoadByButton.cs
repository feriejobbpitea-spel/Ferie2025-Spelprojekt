using UnityEngine;
using UnityEngine.UI;

public class SceneLoadByButton : MonoBehaviour
{
    public Button Button;

    private void OnEnable()
    {
        Button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        string sceneName = Button.name;
        SceneLoader.Instance.LoadScene(sceneName);
    }
}
