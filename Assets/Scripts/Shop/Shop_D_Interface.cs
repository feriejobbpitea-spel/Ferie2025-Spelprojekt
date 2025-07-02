using UnityEngine;
using UnityEngine.Localization;

public interface IShopDialogueHandler
{
    string ShopKeeperName { get; }

    void Initialize();
    void PlayEnterShopDialogue();
    void PlayExitShopDialogue();
    void PlayRandomShopDialogue();
}


[System.Serializable]
public class Dialogue
{
    public LocalizedString LocalizedText;   // Text från localization table
    public AudioClip AudioClip_English;     // Engelskt ljud
    public AudioClip AudioClip_Swedish;     // Svenskt ljud
}