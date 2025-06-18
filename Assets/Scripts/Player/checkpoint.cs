using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool startCheckpoint;
    private Animator _animator;
    private bool _hasReachedCheckpoint = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        // Kontrollera om vi krockar med en fiende
        if (collision.gameObject.CompareTag("Player"))
        {
            SetNewCheckpoint();
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