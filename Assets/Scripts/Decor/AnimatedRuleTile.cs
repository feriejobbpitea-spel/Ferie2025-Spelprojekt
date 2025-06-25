using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[CreateAssetMenu(fileName = "AnimatedRuleTile", menuName = "Tiles/Animated Rule Tile")]
public class AnimatedRuleTile : TileBase
{
    public enum NeighborCondition
    {
        DontCare,
        This,
        NotThis
    }

    [System.Serializable]
    public struct NeighborRule
    {
        public Vector3Int offset;          // Position relative to current tile
        public NeighborCondition condition;
    }
    [System.Serializable]
    public class RuleAnimation
    {
        public Sprite[] frames;
    }

    // You define your own list of rules here
    [System.Serializable]
    public class AnimatedRule
    {
        public Sprite[] sprites; // for preview or static use
        public RuleAnimation animation;
        public List<NeighborRule> neighbors;
    }

    public float Speed = 10F;
    public List<AnimatedRule> rules = new List<AnimatedRule>();
    public Sprite defaultSprite;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector3Int neighborPos = new Vector3Int(position.x + x, position.y + y, position.z);
                tilemap.RefreshTile(neighborPos);
            }
        }
    }


    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var matchingRule = FindMatchingRule(position, tilemap);
        if (matchingRule != null && matchingRule.sprites.Length > 0)
        {
            tileData.sprite = matchingRule.sprites[0];
            tileData.gameObject = null; // or assign your prefab if needed
        }
        else
        {
            tileData.sprite = defaultSprite;
            tileData.gameObject = null;
        }
    }


    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        AnimatedRule matchingRule = FindMatchingRule(position, tilemap);
        if (matchingRule != null && matchingRule.animation != null && matchingRule.animation.frames.Length > 0)
        {
            tileAnimationData.animatedSprites = matchingRule.animation.frames;
            tileAnimationData.animationSpeed = Speed;
            tileAnimationData.animationStartTime = 0f;
            return true;
        }
        return false;
    }

    private AnimatedRule FindMatchingRule(Vector3Int position, ITilemap tilemap)
    {
        foreach (var rule in rules)
        {
            bool ruleMatches = true;

            // Expect 8 neighbors in rule.neighbors, matching top-left, top, top-right, left, right, bottom-left, bottom, bottom-right
            for (int i = 0; i < rule.neighbors.Count; i++)
            {
                var neighbor = rule.neighbors[i];
                Vector3Int neighborPos = position + neighbor.offset;
                TileBase neighborTile = tilemap.GetTile(neighborPos);

                bool isThisTile = neighborTile == this; // Compare if neighbor tile is this same tile

                switch (neighbor.condition)
                {
                    case NeighborCondition.DontCare:
                        // Don't care, no condition, continue
                        break;

                    case NeighborCondition.This:
                        if (!isThisTile)
                        {
                            ruleMatches = false;
                        }
                        break;

                    case NeighborCondition.NotThis:
                        if (isThisTile)
                        {
                            ruleMatches = false;
                        }
                        break;
                }

                if (!ruleMatches)
                    break; // No need to check further neighbors
            }

            if (ruleMatches)
                return rule; // Found matching rule
        }

        return null; // No rule matched
    }

}
