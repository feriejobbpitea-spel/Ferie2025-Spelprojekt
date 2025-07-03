using UnityEngine;

[CreateAssetMenu]
public class Biome_SO : ScriptableObject
{
    public string biomeName = "Grass"; // Default biome name
    public Color backgroundColor = Color.darkOliveGreen;
    public Sprite HeartUI;

    [Header("Parallax Layers")]
    public Color parallaxTint = Color.white;
    public Sprite[] layers;
}
