using UnityEngine;

public class MainMenuEnvironmentLayer : MonoBehaviour
{
    public GameObject tilePrefab;          // Prefab with SpriteRenderer
    public int tileCount = 5;              // Number of tiles to instantiate
    public float moveSpeed = 2f;           // Speed at which tiles move left
    public int SortingCount = 0;

    public Sprite sprite;

    private GameObject[] tiles;
    private float tileWidth;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // Get tile width from prefab's SpriteRenderer
        tileWidth = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        // Instantiate tiles side by side
        tiles = new GameObject[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            Vector3 pos = transform.position + Vector3.right * i * tileWidth;
            tiles[i] = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        float camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect;
        float camRight = cam.transform.position.x + cam.orthographicSize * cam.aspect;

        foreach (GameObject tile in tiles)
        {
            // Move left
            tile.transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            // Cull: disable renderer if outside camera view
            float tileX = tile.transform.position.x;
            bool isVisible = tileX + tileWidth / 2 > camLeft && tileX - tileWidth / 2 < camRight;
            tile.GetComponent<SpriteRenderer>().enabled = isVisible;
            tile.GetComponent<SpriteRenderer>().sprite = sprite;
            tile.GetComponent<SpriteRenderer>().sortingOrder = SortingCount;

            // Recycle: if off screen left, move to rightmost
            if (tileX + tileWidth < camLeft)
            {
                float maxX = GetRightmostTileX();
                tile.transform.position = new Vector3(maxX + tileWidth, tile.transform.position.y, tile.transform.position.z);
            }
        }
    }

    float GetRightmostTileX()
    {
        float maxX = float.MinValue;
        foreach (GameObject tile in tiles)
        {
            if (tile.transform.position.x > maxX)
                maxX = tile.transform.position.x;
        }
        return maxX;
    }
}