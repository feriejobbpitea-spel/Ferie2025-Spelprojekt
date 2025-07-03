using UnityEngine;

public class spikesmovingup : MonoBehaviour
{
    [Header("Inställningar")]
    public float moveHeight = 5f;
    public float moveSpeed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool moveUp = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveHeight;
    }

    void OnEnable()
    {
        PlayerRespawn.OnPlayerRespawn += ResetPlatform;
    }

    void OnDisable()
    {
        PlayerRespawn.OnPlayerRespawn -= ResetPlatform;
    }

    void Update()
    {
        if (moveUp && transform.position.y < targetPos.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    public void ActivatePlatform()
    {
        moveUp = true;
    }

    private void ResetPlatform()
    {
        transform.position = startPos;
        moveUp = false;
    }
}