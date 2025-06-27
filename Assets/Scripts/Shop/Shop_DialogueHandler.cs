using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

[System.Serializable]
public class Dialogue
{
    public LocalizedString LocalizedText;   // Text från localization table
    public AudioClip AudioClip_English;     // Engelskt ljud
    public AudioClip AudioClip_Swedish;     // Svenskt ljud
}

[System.Serializable]

public class Shop_DialogueHandler
{
    public string ShopKeeperName = "John";
    public Dialogue[] EnterShop;
    public Dialogue[] ExitShop;
    public Dialogue[] RandomDialogue;

    public Button TalkButton;

    public void Initialize()
    {
        if (TalkButton != null)
        {
            TalkButton.onClick.AddListener(PlayRandomShopDialogue);
        }
    }

    public void PlayEnterShopDialogue()
    {
        var dialogue = EnterShop[Random.Range(0, EnterShop.Length)];
        ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
    }

    public void PlayExitShopDialogue()
    {
        var dialogue = ExitShop[Random.Range(0, ExitShop.Length)];
        ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
    }

    public void PlayRandomShopDialogue()
    {
        var dialogue = RandomDialogue[Random.Range(0, RandomDialogue.Length)];
        ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
    }

    private IEnumerator PlayLocalizedDialogue(Dialogue dialogue)
    {
        var localizedTextOp = dialogue.LocalizedText.GetLocalizedStringAsync();
        yield return localizedTextOp;
        string text = localizedTextOp.Result;

        AudioClip clipToPlay;

        // Kolla valt språk (Locale-kod är t.ex. "sv" eller "en")
        var currentLocale = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;

        if (currentLocale == "sv")
            clipToPlay = dialogue.AudioClip_Swedish;
        else
            clipToPlay = dialogue.AudioClip_English;

        ShopDialogueManager.Instance.NewDialogue(ShopKeeperName, text, clipToPlay);
    }
}