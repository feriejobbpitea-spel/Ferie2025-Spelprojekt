using UnityEngine;

public class ParallaxManager : Singleton<ParallaxManager>
{
    public ParallaxLayer parallaxLayerPrefab;
    public float minParallaxStrength = 0.02f;
    public float maxParallaxStrength = 0.2f;

    public void UpdateParallaxSprites(Biome_SO biome)
    {
        for (int i = 0; i < biome.layers.Length; i++)
        {
            var item = biome.layers[i];

            ParallaxLayer newLayer = GameObject.Instantiate(parallaxLayerPrefab, transform);
            newLayer.transform.SetParent(transform);
            newLayer.SetSprite(item, biome.parallaxTint);
            float t = (biome.layers.Length > 1) ? (float)i / (biome.layers.Length - 1) : 0f;

            // Interpolate between max and min strength (max for layer 0, min for last layer)
            newLayer.parallaxFactor = Mathf.Lerp(maxParallaxStrength, minParallaxStrength, t);
            newLayer.SetSortingLayer(-100 - i);
        }
        /*
        if (biome.layer1 != null)
            parallaxLayers[0].SetSprite(biome.layer1, biome.parallaxTint);
        if(biome.layer2 != null)
            parallaxLayers[1].SetSprite(biome.layer2, biome.parallaxTint);
        if (biome.layer3 != null)
            parallaxLayers[2].SetSprite(biome.layer3, biome.parallaxTint);*/
    }
}
