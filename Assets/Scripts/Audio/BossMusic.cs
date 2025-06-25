using System.Collections;
using UnityEngine;

public class BossMusic : MonoBehaviour
{
    [SerializeField] private AudioClip Intro;
    [SerializeField] private AudioClip MainTheme;
    [SerializeField] private float StartingRange;
    private AudioSource _audioSource;
    private Transform _player;


    private IEnumerator Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component not found on this GameObject.");
            yield break;
        }

        yield return new WaitUntil(() => Vector3.Distance(transform.position, _player.transform.position) < StartingRange);

        // Spela intro
        _audioSource.clip = Intro;
        _audioSource.Play();
        yield return new WaitForSeconds(Intro.length);
        // Spela huvudtemat
        _audioSource.clip = MainTheme;
        _audioSource.loop = true; // Sätt loop för huvudtemat
        _audioSource.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, StartingRange);
    }
}
