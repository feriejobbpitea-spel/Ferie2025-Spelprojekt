using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;


[System.Serializable]

public class Shop_DialogueHandler : IShopDialogueHandler
{
    public string ShopKeeperName = "John";
    public Dialogue[] EnterShop;
    public Dialogue[] ExitShop;
    public Dialogue[] RandomDialogue;
    public Dialogue[] BuyItem; 

    public Dialogue[] AlternativeEnterShop;
    public Dialogue[] AlternativeExitShop;
    public Dialogue[] AlternativeRandomDialogue;

    public Button TalkButton;
    public bool UseAlternativeRandomDialogue = false;

    string IShopDialogueHandler.ShopKeeperName => ShopKeeperName;

    public void Initialize()
    {
        if (TalkButton != null)
        {
            TalkButton.onClick.AddListener(PlayRandomShopDialogue);
        }
    }

    public void PlayBuyItem()
    {
        if (BuyItem.Length > 0)
        {
            var dialogue = BuyItem[Random.Range(0, BuyItem.Length)];
            ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
        }
    }
    public void PlayEnterShopDialogue()
    {
        Dialogue[] dialoguesToUse = UseAlternativeRandomDialogue ? AlternativeEnterShop : EnterShop;
        string dialogueType = UseAlternativeRandomDialogue ? "alternative" : "standard";
        if (dialoguesToUse.Length == 0)
        {
            Debug.LogWarning($"No {dialogueType} enter dialogue available.");
            return;
        }

        var dialogue = dialoguesToUse[Random.Range(0, dialoguesToUse.Length)];
        ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
    }

    public void PlayExitShopDialogue()
    {
        Dialogue[] dialoguesToUse = UseAlternativeRandomDialogue ? AlternativeExitShop : ExitShop;
        string dialogueType = UseAlternativeRandomDialogue ? "alternative" : "standard";
        if (dialoguesToUse.Length == 0)
        {
            Debug.LogWarning($"No {dialogueType} exit dialogue available.");
            return;
        }

        var dialogue = dialoguesToUse[Random.Range(0, dialoguesToUse.Length)];
        ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
    }

    public void PlayRandomShopDialogue()
    {
        if (UseAlternativeRandomDialogue)
        {
            var dialogue = AlternativeRandomDialogue[Random.Range(0, AlternativeRandomDialogue.Length)];
            ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
        }
        else
        {
            var dialogue = RandomDialogue[Random.Range(0, RandomDialogue.Length)];
            ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
        }
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