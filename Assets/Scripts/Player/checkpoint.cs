using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public bool startCheckpoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {if (startCheckpoint)
        {
            // Om detta är startcheckpoint, sätt den som respawnpunkt
            PlayerRespawn.Instance.SetCheckpoint(transform.position);
            Debug.Log("Start checkpoint set at: " + transform.position);
        }
       

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kontrollera om vi krockar med en fiende
        if (collision.gameObject.CompareTag("Player"))
        {
            setnewcheckpoint();
        }
    }
    
        
    
    void setnewcheckpoint()
    {
        Debug.Log("Player set new checkpoint");
        PlayerRespawn.Instance.SetCheckpoint(transform.position);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
