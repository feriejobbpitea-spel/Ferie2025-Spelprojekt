using UnityEngine;

public class Enemy_01 : MonoBehaviour
{
    public float moveSpeed = 2f; // Hastighet p� fiendens r�relse
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D saknas p� fienden!");
        }
    }

    void FixedUpdate()
    {
        // S�tt fiendens hastighet �t v�nster
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
    }
}



   
        
    

    
        
    

