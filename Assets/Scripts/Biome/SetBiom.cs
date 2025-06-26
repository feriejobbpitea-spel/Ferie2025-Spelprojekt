using UnityEngine;

public class SetBiom : MonoBehaviour
{
    
    public GameObject biomeM;
    public GameObject biomeB;
    public string biome = "Grass"; // Standardbiom, kan �ndras i inspektorn
    private string lastBiome = "Grass"; // F�r att h�lla koll p� senaste biomet
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         string lastBiome = biome;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        
        float playerX = transform.position.x;
        float playerY = transform.position.y;
        float biomeMX = biomeM.transform.position.x;
        float biomeMY = biomeM.transform.position.y;
        float biomeBX = biomeB.transform.position.x;
        float biomeBY = biomeB.transform.position.y;
        
         if (playerX > biomeMX && playerY < biomeMY)
        {
             biome = "mine"; 
           
        } else if(playerX>biomeBX) { biome = "boss"; } else { biome = "Grass"; } // Om spelaren �r i gr�sbiomet, s�tt biome till "Grass"

        if (lastBiome != biome)
        {
            player.GetComponent<PlayerHealthV2>().UpdateHearts();
            lastBiome = biome;

        }



    }
}
