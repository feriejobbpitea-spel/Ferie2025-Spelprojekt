using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BossHealth : EnemyHealth
{
    public Animator Animator;
    public string GameCompleteScene = "GameComplete"; // Name of the scene to load on death    

    [Header("Voicelines - Death")]
    public List<LocalizedAudioClip> deathVoicelines = new List<LocalizedAudioClip>();

    private bool isDead = false;
    private AudioSource voiceAudioSource; // Separat AudioSource för tal/ljud

    private void Awake()
    {
        voiceAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void Die()
    {
        if (isDead)
            return;
        isDead = true;

        PlayRandomDeathVoiceline();

        Animator.SetTrigger("Die");
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(6);
        SceneLoader.Instance.LoadScene(GameCompleteScene);
    }

    private void PlayRandomDeathVoiceline()
    {
        PlayRandomVoicelineFromList(deathVoicelines);
    }

    private void PlayRandomVoicelineFromList(List<LocalizedAudioClip> voicelines)
    {
        if (voicelines == null || voicelines.Count == 0)
        {
            return;
        }

        LocalizedAudioClip clipToPlay = voicelines[UnityEngine.Random.Range(0, voicelines.Count)];

        var handle = clipToPlay.LoadAssetAsync();
        handle.Completed += (AsyncOperationHandle<AudioClip> op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded && op.Result != null)
            {
                voiceAudioSource.PlayOneShot(op.Result);
            }
            else
            {
                Debug.LogError($"Failed to load voiceline from {clipToPlay}");
            }
        };
    }
}