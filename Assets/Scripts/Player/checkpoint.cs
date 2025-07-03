using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool startCheckpoint;
    public AudioClip checkpointSound; // Lägg till detta i Unity-editorn
    private Animator _animator;
    private bool _hasReachedCheckpoint = false;
    private AudioSource _audioSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        // Om ingen AudioSource är satt, lägg till en dynamiskt
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (startCheckpoint)
        {
            SetNewCheckpoint();
            _animator.SetTrigger("Activate");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_hasReachedCheckpoint) 
            {
                if (checkpointSound != null)
                {
                    _audioSource.PlayOneShot(checkpointSound);
                }
                SetNewCheckpoint();
                PlayerHealthV2.Instance.AddSingleLife();
            }
            
        }
    }

    void SetNewCheckpoint()
    {
        if (_hasReachedCheckpoint)
            return;

        _hasReachedCheckpoint = true;
        Debug.Log("Player set new checkpoint");
        PlayerRespawn.Instance.SetCheckpoint(transform.position);
        _animator.SetTrigger("Activate");
    }
}
