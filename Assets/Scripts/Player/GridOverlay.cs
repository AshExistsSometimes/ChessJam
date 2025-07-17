using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Player reference
    public GameObject gridTilePrefab; // The transparent tile prefab

    [Header("Settings")]
    public int radius = 5; // How far the grid extends
    public float fadeDistance = 5f; // Distance at which grid tiles fully fade
    [Range(0, 255)]
    public float maximumAlpha = 80f; // Slider to control max alpha (0–255)

    private readonly List<GameObject> gridTiles = new();

    public static GridOverlay Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RefreshGrid()
    {
        // Clean up old grid tiles
        foreach (GameObject tile in gridTiles)
        {
            Destroy(tile);
        }
        gridTiles.Clear();

        Vector3 center = player.position;
        Vector3Int centerInt = Vector3Int.RoundToInt(center);

        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                // Skip every other tile to make a checkerboard pattern
                if ((x + z) % 2 != 0)
                    continue;

                Vector3 worldPos = new Vector3(centerInt.x + x, centerInt.y, centerInt.z + z);

                float distance = Vector3.Distance(center, worldPos);
                if (distance > radius) continue; // Optional: circular radius cutoff

                GameObject tile = Instantiate(gridTilePrefab, worldPos, Quaternion.identity, transform);
                tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Lay it flat
                SetAlpha(tile, 1f - (distance / fadeDistance));
                gridTiles.Add(tile);
            }
        }
    }

    void SetAlpha(GameObject tile, float normalizedAlpha)
    {
        Renderer rend = tile.GetComponent<Renderer>();
        if (rend != null && rend.material.HasProperty("_Color"))
        {
            // Convert 0–1 alpha to 0–255 range, clamp to max, then convert back to 0–1
            float clampedAlpha = Mathf.Min(normalizedAlpha * 255f, maximumAlpha) / 255f;

            Color color = rend.material.color;
            color.a = clampedAlpha;
            rend.material.color = color;
        }
    }
}
