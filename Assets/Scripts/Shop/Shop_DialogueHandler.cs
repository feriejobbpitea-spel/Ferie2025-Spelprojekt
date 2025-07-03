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
    public Dialogue[] AlternativeRandomDialogue;
    public Dialogue[] RandomDialogue;
    public Dialogue[] BuyItem; 

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
        var dialogue = BuyItem[Random.Range(0, BuyItem.Length)];
        ShopDialogueManager.Instance.StartCoroutine(PlayLocalizedDialogue(dialogue));
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