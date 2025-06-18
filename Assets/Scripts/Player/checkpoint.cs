using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool startCheckpoint;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (startCheckpoint)
        {
            // Om detta är startcheckpoint, sätt den som respawnpunkt
            SetNewCheckpoint();
            Debug.Log("Start checkpoint set at: " + transform.position);
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
        Debug.Log("Player set new checkpoint");
        PlayerRespawn.Instance.SetCheckpoint(transform.position);
        _animator.SetTrigger("Activate");
    }
}