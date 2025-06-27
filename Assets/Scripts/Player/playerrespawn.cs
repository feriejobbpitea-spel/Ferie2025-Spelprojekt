using UnityEngine;

public class PlayerRespawn : Singleton<PlayerRespawn>
{
    private Vector3 respawnPosition;
    [SerializeField] private Vector3 superCheckpointPosition;

    public static event System.Action OnPlayerRespawn;

    void Start()
    {
        respawnPosition = transform.position;
        superCheckpointPosition = GameObject.FindGameObjectWithTag("Player")?.transform.position ?? Vector3.zero;
    }

    public void Respawn()
    {
        transform.position = respawnPosition;
        Debug.Log("Respawnar vid vanlig checkpoint...");

        OnPlayerRespawn?.Invoke();
    }

    public void RespawnAtSuperCheckpoint()
    {
        transform.position = superCheckpointPosition;
        Debug.Log("Respawnar vid SUPER checkpoint!");

        OnPlayerRespawn?.Invoke();
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPosition = newCheckpoint;
        Debug.Log("Ny checkpoint satt.");
    }

    public void SetSuperCheckpoint(Vector3 newCheckpoint)
    {
        superCheckpointPosition = newCheckpoint;
        Debug.Log("Super checkpoint satt.");
    }
}