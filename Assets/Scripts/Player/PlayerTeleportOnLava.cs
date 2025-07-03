using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTeleportOnLava : MonoBehaviour
{
    [Header("Tilemap with platforms")]
    public Tilemap groundTilemap;

    [Header("Teleport settings")]
    public float maxSearchHeight = 15f;   // Hur högt upp vi söker efter tiles
    public int searchWidth = 5;            // Hur brett horisontellt vi söker (antal tiles)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"OnTriggerEnter2D: {collision.gameObject.name} ({collision.gameObject.tag})");
        if (collision.CompareTag("Lava"))
        {
            TeleportToNearestPlatformAbove();
        }
    }

    private void TeleportToNearestPlatformAbove()
    {
        Vector3 playerPos = transform.position;
        Vector3Int playerCell = groundTilemap.WorldToCell(playerPos);

        float bestDistance = float.MaxValue;
        Vector3Int bestCell = playerCell;
        bool foundTile = false;

        for (int y = playerCell.y + 1; y <= playerCell.y + maxSearchHeight; y++)
        {
            for (int x = playerCell.x - searchWidth / 2; x <= playerCell.x + searchWidth / 2; x++)
            {
                Vector3Int checkCell = new Vector3Int(x, y, playerCell.z);
                TileBase tile = groundTilemap.GetTile(checkCell);
                if (tile != null)
                {
                    Vector3 tileWorldPos = groundTilemap.GetCellCenterWorld(checkCell);
                    float distance = Vector2.Distance(new Vector2(playerPos.x, playerPos.y), new Vector2(tileWorldPos.x, tileWorldPos.y));

                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestCell = checkCell;
                        foundTile = true;
                    }
                }
            }
        }

        if (!foundTile)
        {
            Debug.LogWarning("Ingen plattform (tile) hittades ovanför spelaren.");
            return;
        }

        Vector3 teleportPos = groundTilemap.GetCellCenterWorld(bestCell);
        teleportPos.y += groundTilemap.cellSize.y / 2f + 0.1f;  // Lite ovanför tile

        transform.position = teleportPos;

        Debug.Log($"Teleporterad till plattform vid cell {bestCell} (värld: {teleportPos})");
    }
}
