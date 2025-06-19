using UnityEngine;
using UnityEngine.UI;

public class SceneLoadByButton : MonoBehaviour
{
    public Button Button;
    public string SceneName;

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
        SceneLoader.Instance.LoadScene(SceneName);
    }
}
