using UnityEngine;

public class ParallaxManager : Singleton<ParallaxManager>
{
    public ParallaxLayer[] parallaxLayers; // should be ordered: 0 = farthest, 2 = closest

    public void UpdateParallaxSprites(Biome_SO biome)
    {
        if (biome.layer1 != null)
            parallaxLayers[0].SetSprite(biome.layer1);
        if(biome.layer2 != null)
            parallaxLayers[1].SetSprite(biome.layer2);
        if (biome.layer3 != null)
            parallaxLayers[2].SetSprite(biome.layer3);
    }
}
