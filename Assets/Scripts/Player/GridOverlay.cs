using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        {
            Destroy(gameObject);
            return;
        }

        TryAssignPlayer();

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "Main_Menu")
        {
            RefreshGrid();
        }
    }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Clean up tiles if on Main Menu
        if (currentScene == "Main_Menu")
        {
            foreach (GameObject tile in gridTiles)
            {
                Destroy(tile);
            }
            gridTiles.Clear();
            return;
        }

        // Reassign player if lost
        if (player == null)
        {
            TryAssignPlayer();
            if (player != null)
            {
                RefreshGrid();
            }
        }
    }

    void TryAssignPlayer()
    {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        if (pm != null)
        {
            player = pm.transform;
        }
    }

    public void RefreshGrid()
    {
        if (player == null)
            return;

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
                if ((x + z) % 2 != 0)
                    continue;

                Vector3 worldPos = new Vector3(centerInt.x + x, centerInt.y, centerInt.z + z);

                float distance = Vector3.Distance(center, worldPos);
                if (distance > radius) continue;

                GameObject tile = Instantiate(gridTilePrefab, worldPos, Quaternion.identity, transform);
                tile.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
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
            float clampedAlpha = Mathf.Min(normalizedAlpha * 255f, maximumAlpha) / 255f;
            Color color = rend.material.color;
            color.a = clampedAlpha;
            rend.material.color = color;
        }
    }
}
