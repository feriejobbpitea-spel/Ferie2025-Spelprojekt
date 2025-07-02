using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor = 0.5f;
    public Transform cameraTransform;
    public GameObject tilePrefab;
    public int SortingOrder = -10;
    public float Scale = 10;
    public float TransitionDuration = 0.5f; // Duration for sprite transition

    private float spriteWidth;
    private List<Transform> tiles = new List<Transform>();
    private Vector3 lastCameraPosition;

    private void Awake()
    {
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;

        lastCameraPosition = cameraTransform.position;

        SpriteRenderer sr = tilePrefab.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Tile prefab must have a SpriteRenderer with a sprite.");
            return;
        }

        spriteWidth = sr.bounds.size.x * Scale;

        // Calculate how many tiles are needed to cover screen width + buffer
        float screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        int neededTiles = Mathf.CeilToInt(screenWidth / spriteWidth) + 2;


        // Create tiles centered around camera
        for (int i = -neededTiles / 2; i <= neededTiles / 2; i++)
        {
            GameObject tile = Instantiate(tilePrefab, transform);
            tile.transform.localScale = Vector3.one * Scale;

            tile.GetComponent<SpriteRenderer>().sprite = null;
            tile.transform.position = new Vector3(
                cameraTransform.position.x + i * spriteWidth,
                transform.position.y,
                transform.position.z
            );
            tiles.Add(tile.transform);
        }
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        lastCameraPosition = cameraTransform.position;

        float camLeftEdge = cameraTransform.position.x - Camera.main.orthographicSize * Camera.main.aspect;
        float camRightEdge = cameraTransform.position.x + Camera.main.orthographicSize * Camera.main.aspect;

        for (int i = 0; i < tiles.Count; i++)
        {
            Transform tile = tiles[i];

            // Apply parallax movement
            tile.position += new Vector3(deltaMovement.x * parallaxFactor, 0f, 0f);

            // Always match camera Y
            tile.position = new Vector3(tile.position.x, cameraTransform.position.y, tile.position.z);

            // Check horizontal wrapping
            if (tile.position.x + spriteWidth < camLeftEdge)
            {
                float rightMostX = GetRightMostTileX();
                tile.position = new Vector3(rightMostX + spriteWidth, cameraTransform.position.y, tile.position.z);
            }
            else if (tile.position.x - spriteWidth > camRightEdge)
            {
                float leftMostX = GetLeftMostTileX();
                tile.position = new Vector3(leftMostX - spriteWidth, cameraTransform.position.y, tile.position.z);
            }
        }
    }


    private float GetLeftMostTileX()
    {
        float minX = float.MaxValue;
        foreach (var t in tiles)
            if (t.position.x < minX)
                minX = t.position.x;
        return minX;
    }

    private float GetRightMostTileX()
    {
        float maxX = float.MinValue;
        foreach (var t in tiles)
            if (t.position.x > maxX)
                maxX = t.position.x;
        return maxX;
    }


    public void SetSprite(Sprite newSprite, Color tint)
    {
        spriteWidth = newSprite.bounds.size.x * Scale;

        for (int i = 0; i < tiles.Count; i++)
        {
            Transform tile = tiles[i];
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

            if (sr != null && newSprite != null)
            {
                // Setup fade-out of old sprite
                if (sr.sprite != null)
                {
                    GameObject fadeObj = new GameObject("FadeSprite");
                    fadeObj.transform.SetParent(tile);
                    fadeObj.transform.localPosition = Vector3.zero;
                    fadeObj.transform.localRotation = Quaternion.identity;
                    fadeObj.transform.localScale = Vector3.one;

                    SpriteRenderer fadeSr = fadeObj.AddComponent<SpriteRenderer>();
                    fadeSr.sprite = sr.sprite;
                    fadeSr.sortingOrder = sr.sortingOrder - 1;
                    fadeSr.flipX = sr.flipX;
                    fadeSr.color = sr.color;

                    fadeSr.DOFade(0, TransitionDuration).OnComplete(() => Destroy(fadeObj));
                }

                // Fade in new sprite
                sr.color = new Color(tint.r, tint.g, tint.b, 0);
                sr.sprite = newSprite;
                sr.sortingOrder = SortingOrder;
                sr.flipX = (i % 2 != 0);

                sr.DOFade(tint.a, TransitionDuration); // fade in
            }



            tile.localScale = Vector3.one * Scale;
        }
    }


}
