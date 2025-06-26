using UnityEngine;

public class PlayerRespawn : Singleton<PlayerRespawn>
{
    private Vector3 respawnPosition;
    [SerializeField] private Vector3 superCheckpointPosition;

    void Start()
    {
        // Första respawnpositionen är spelarens startplats
        respawnPosition = transform.position;
    }

    /// <summary>
    /// Respawnar vid senaste vanliga checkpointen.
    /// </summary>
    public void Respawn()
    {
        transform.position = respawnPosition;
        Debug.Log("Respawnar vid vanlig checkpoint...");
    }

    /// <summary>
    /// Respawnar vid super checkpoint. Anropas ex. efter game over.
    /// </summary>
    public void RespawnAtSuperCheckpoint()
    {
        transform.position = superCheckpointPosition;
        Debug.Log("Respawnar vid SUPER checkpoint!");
    }

    /// <summary>
    /// Sätter ny vanlig checkpoint.
    /// </summary>
    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPosition = newCheckpoint;
        Debug.Log("Ny checkpoint satt.");
    }

    /// <summary>
    /// Sätter super checkpoint via kod om du inte använder inspector.
    /// </summary>
    public void SetSuperCheckpoint(Vector3 superCheckpoint)
    {
        superCheckpointPosition = superCheckpoint;
        Debug.Log("Super checkpoint satt.");
    }
}
