using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Dialogue 
{
    public string Text;
    public AudioClip AudioClip;
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
        var randomShopkeeperDialogue = EnterShop[UnityEngine.Random.Range(0, EnterShop.Length)];
        ShopDialogueManager.Instance.NewDialogue(ShopKeeperName, randomShopkeeperDialogue.Text, randomShopkeeperDialogue.AudioClip);
    }

    public void PlayExitShopDialogue()
    {
        var randomShopkeeperDialogue = ExitShop[UnityEngine.Random.Range(0, ExitShop.Length)];
        ShopDialogueManager.Instance.NewDialogue(ShopKeeperName, randomShopkeeperDialogue.Text, randomShopkeeperDialogue.AudioClip);
    }

    public void PlayRandomShopDialogue()
    {
        var randomShopkeeperDialogue = RandomDialogue[UnityEngine.Random.Range(0, RandomDialogue.Length)];
        ShopDialogueManager.Instance.NewDialogue(ShopKeeperName, randomShopkeeperDialogue.Text, randomShopkeeperDialogue.AudioClip);
    }
}
