using UnityEngine;

[CreateAssetMenu]
public class Biome_SO : ScriptableObject
{
    public string biomeName = "Grass"; // Default biome name
    public Color backgroundColor = Color.darkOliveGreen;
    public Sprite HeartUI;

    [Header("Parallax Layers")]
    public Sprite layer1; // furthest (e.g. mountains)
    public Sprite layer2; // middle (e.g. trees)
    public Sprite layer3; // closest (e.g. bushes)
}
