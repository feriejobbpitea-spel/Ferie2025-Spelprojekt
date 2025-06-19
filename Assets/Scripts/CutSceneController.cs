using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CutsceneSlide
{
    public Sprite image;
    [TextArea(2, 5)]
    public string text;
}

public class CutSceneController : MonoBehaviour
{
    public CutsceneSlide[] slides;
    public Image cutsceneImage;
    public TextMeshProUGUI cutsceneText;

    private int currentSlideIndex = 0;

    void Start()
    {
        ShowSlide(currentSlideIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // ENTER-tangent
        {
            currentSlideIndex++;
            if (currentSlideIndex < slides.Length)
            {
                ShowSlide(currentSlideIndex);
            }
            else
            {
                // Byt scen här när cutscenen är klar
                SceneManager.LoadScene("Level1"); // <-- byt till rätt scen-namn!
            }
        }
    }

    void ShowSlide(int index)
    {
        cutsceneImage.sprite = slides[index].image;
        cutsceneText.text = slides[index].text;
    }
}
