using UnityEngine;

public class ParallaxManager : Singleton<ParallaxManager>
{
    public ParallaxLayer parallaxLayerPrefab;

    public void UpdateParallaxSprites(Biome_SO biome)
    {
        for (int i = 0; i < biome.layers.Length; i++)
        {
            var item = biome.layers[i];

            ParallaxLayer newLayer = GameObject.Instantiate(parallaxLayerPrefab, transform);
            newLayer.transform.SetParent(transform);
            newLayer.SetSprite(item, biome.parallaxTint);
            newLayer.parallaxFactor = (0.7f / i); // Adjust parallax factor based on layer index
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
