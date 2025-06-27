using UnityEngine;

public class earthwave : MonoBehaviour
{
    private Transform playerTransform;
    private Vector2 playerDirection;

    public float speed = 5f;
    public int damage = 1;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Se till att spelaren har taggen 'Player'.");
            playerDirection = Vector2.right; // Standardriktning om ingen spelare finns
        }
        else
        {
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            dir.y = 0; // Endast horisontell riktning
            playerDirection = dir;
        }
        Destroy(gameObject, 5f); // Förstör vågen efter 5 sekunder
    }

    private void Update()
    {
        transform.Translate(playerDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Skada spelaren (byt ut enligt din skademetod)
            PlayerHealthV2.Instance.LoseLife();
            Destroy(gameObject);
            
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}