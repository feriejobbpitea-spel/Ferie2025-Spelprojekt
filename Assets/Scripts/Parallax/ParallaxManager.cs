using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ParallaxManager : Singleton<ParallaxManager>
{
    public ParallaxLayer parallaxLayerPrefab;
    public float minParallaxStrength = 0.02f;
    public float maxParallaxStrength = 0.2f;

    private List<ParallaxLayer> parallaxLayers = new();

    public void UpdateParallaxSprites(Biome_SO biome)
    {
        int i;
        for (i = 0; i < biome.layers.Length; i++)
        {
            var item = biome.layers[i];
            float t = (biome.layers.Length > 1) ? (float)i / (biome.layers.Length - 1) : 0f;

            ParallaxLayer layer = null;

            if (i < parallaxLayers.Count)
            {
                layer = parallaxLayers[i];
            }
            else
            {
                layer = GameObject.Instantiate(parallaxLayerPrefab, transform);
                parallaxLayers.Add(layer);
            }

            layer.transform.SetParent(transform);
            layer.SetSprite(item, biome.parallaxTint);

            // Interpolate between max and min strength (max for layer 0, min for last layer)
            layer.parallaxFactor = Mathf.Lerp(maxParallaxStrength, minParallaxStrength, t);
            layer.SetSortingLayer(-100 - i);
        }

        // Clear sprites on any unused layers
        for (; i < parallaxLayers.Count; i++)
        {
            parallaxLayers[i].SetSprite(null, Color.white);  // or your default color
        }
    }

}
