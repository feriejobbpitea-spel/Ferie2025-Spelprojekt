using UnityEngine;

public class BiomeTrigger : MonoBehaviour
{
    public Biome_SO Biome;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BiomeHandler.Instance.UpdateBiome(Biome);
        }
    }
}
