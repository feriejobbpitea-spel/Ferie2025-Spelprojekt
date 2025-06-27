using UnityEngine;

public class earthwave : MonoBehaviour
{
    public LayerMask GroundLayer;
    public LayerMask PlayerLayer;
    public float RemoveAfterTime = 5F;
    public float DistanceToWalls = 0.1F;

    private void Start()
    {
        Destroy(gameObject, RemoveAfterTime); // Förstör vågen efter 5 sekunder
    }

    private void Update()
    {
        bool hitGroundRight = Physics2D.Raycast(transform.position, Vector2.right, DistanceToWalls, GroundLayer);
        bool hitGroundLeft = Physics2D.Raycast(transform.position, Vector2.right, DistanceToWalls, GroundLayer);
        if(hitGroundLeft || hitGroundRight)
        {
            RemoveSelf();
        }

        bool hitPlayer = Physics2D.CircleCast(transform.position, 1F, Vector2.up, 1F, PlayerLayer);
        if (hitPlayer)
        {
            RemoveSelf();
            HurtPlayer();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1F); // Visar cirkeln för spelarkollision
        Gizmos.DrawRay(transform.position, Vector2.right * DistanceToWalls); // Visar raycast för markkollision höger
        Gizmos.DrawRay(transform.position, Vector2.left * DistanceToWalls); // Visar raycast för markkollision vänster
    }


    private void RemoveSelf() {
        Destroy(gameObject);
    }

    private void HurtPlayer() 
    {
        PlayerHealthV2.Instance.LoseLife();
    }


    /*    private void OnTriggerEnter2D(Collider2D collision)
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
        }*/
}