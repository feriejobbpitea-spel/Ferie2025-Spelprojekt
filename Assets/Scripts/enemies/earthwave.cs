using UnityEngine;

public class earthwave : MonoBehaviour
{
    Transform playerTransform; // Referens till spelarens transform
    Vector2 Playerdirection;
    private void Start()
    {playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Hitta spelaren i scenen
        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
        else
        {
            // Justera riktning mot spelaren om det beh�vs
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            Playerdirection = direction;
            Playerdirection.y = 0; // S�tt y-komponenten till 0 f�r att h�lla den horisontell
        }

    }
    public float speed = 5f;
    public int damage = 1;

    void Update()
    {
        transform.Translate(Playerdirection * speed * Time.deltaTime); // Justera riktning vid behov
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth.Instance.LoseLife();
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject); // Stoppa n�r den tr�ffar n�got i milj�n
        }
    }
}
