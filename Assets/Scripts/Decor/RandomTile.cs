using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Tiles/Random Tile with Bias")]
public class RandomTile : Tile
{
    [System.Serializable]
    public struct WeightedSprite
    {
        public Sprite sprite;
        [Range(0f, 1f)]
        public float weight;
    }

    public WeightedSprite[] weightedSprites;

    private Sprite GetWeightedRandomSprite(Vector3Int position)
    {
        float totalWeight = weightedSprites.Sum(ws => ws.weight);
        if (totalWeight <= 0f) return null;

        float hash = Mathf.Abs((position.x * 73856093 ^ position.y * 19349663) % 1000) / 1000f;
        float randomValue = hash * totalWeight;

        float cumulative = 0f;
        foreach (var ws in weightedSprites)
        {
            cumulative += ws.weight;
            if (randomValue <= cumulative)
                return ws.sprite;
        }

        // Fallback
        return weightedSprites[weightedSprites.Length - 1].sprite;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var sprite = GetWeightedRandomSprite(position);
        tileData.sprite = sprite;
        tileData.color = Color.white;
        tileData.transform = Matrix4x4.identity;
        tileData.flags = TileFlags.LockTransform;
        tileData.colliderType = colliderType;
    }
}
