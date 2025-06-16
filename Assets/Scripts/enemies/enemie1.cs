using UnityEngine;

public class enemie1 : MonoBehaviour
{
    public float moveSpeed = 2f; // Hastighet på fiendens rörelse
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D saknas på fienden!");
        }
    }

    void FixedUpdate()
    {
        // Sätt fiendens hastighet åt vänster
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
    }
}



   
        
    

    
        
    

