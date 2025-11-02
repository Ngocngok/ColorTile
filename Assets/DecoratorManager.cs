using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecoratorManager : MonoBehaviour
{
    [Header("Decorator Settings")]
    [Tooltip("Global scale multiplier for all decorators")]
    public float decoratorScale = .8f;
    
    [Tooltip("Minimum number of decorators to spawn")]
    public int minDecorators = 15;
    
    [Tooltip("Maximum number of decorators to spawn")]
    public int maxDecorators = 22;
    
    [Tooltip("Minimum distance from tile edges (buffer zone)")]
    public float minDistanceFromTiles = 2f;
    
    [Tooltip("Spawn area extension - Left/Right/Bottom")]
    public float spawnAreaExtension = 8f;
    
    [Tooltip("Spawn area extension - Top (can be larger for mobile)")]
    public float spawnAreaExtensionTop = 12f;

    [Header("Decorator Prefabs (FBX)")]
    public GameObject barrierStrut;
    public GameObject flag_teamRed;
    public GameObject flag_teamYellow;
    public GameObject plantB_forest;
    public GameObject spikeRoller;
    public GameObject targetStand;
    public GameObject tree_forest;
    public GameObject tree_desert;

    private GameObject decoratorParent;
    private GameObject[] decoratorPrefabs;

    void Awake()
    {
        // Create parent object for decorators
        decoratorParent = new GameObject("Decorators");
        decoratorParent.transform.SetParent(transform);
    }

    public void SpawnDecorators(int gridWidth, int gridHeight, float tileSize)
    {
        // Clear existing decorators
        ClearDecorators();

        // Build array of available prefabs
        decoratorPrefabs = new GameObject[]
        {
            barrierStrut,
            flag_teamRed,
            flag_teamYellow,
            plantB_forest,
            spikeRoller,
            targetStand,
            tree_forest,
            tree_desert
        };

        // Remove null entries
        System.Collections.Generic.List<GameObject> validPrefabs = new System.Collections.Generic.List<GameObject>();
        foreach (GameObject prefab in decoratorPrefabs)
        {
            if (prefab != null)
            {
                validPrefabs.Add(prefab);
            }
        }

        if (validPrefabs.Count == 0)
        {
            Debug.LogWarning("No decorator prefabs assigned!");
            return;
        }

        // Calculate map bounds
        float mapWidth = gridWidth * tileSize;
        float mapHeight = gridHeight * tileSize;
        float centerX = mapWidth / 2f - tileSize / 2f;

        // Determine number of decorators to spawn
        int decoratorCount = Random.Range(minDecorators, maxDecorators + 1);

        // Define the total spawn area (including border around tiles)
        float spawnAreaMinX = -spawnAreaExtension;
        float spawnAreaMaxX = mapWidth + spawnAreaExtension;
        float spawnAreaMinZ = -spawnAreaExtension;
        float spawnAreaMaxZ = mapHeight + spawnAreaExtensionTop;

        // Define the tile area (where decorators should NOT spawn)
        float tileAreaMinX = -minDistanceFromTiles;
        float tileAreaMaxX = mapWidth + minDistanceFromTiles;
        float tileAreaMinZ = -minDistanceFromTiles;
        float tileAreaMaxZ = mapHeight + minDistanceFromTiles;

        for (int i = 0; i < decoratorCount; i++)
        {
            // Randomly choose a prefab
            GameObject prefab = validPrefabs[Random.Range(0, validPrefabs.Count)];

            // Try to find a valid spawn position (not in tile area)
            Vector3 spawnPosition = Vector3.zero;
            bool validPositionFound = false;
            int maxAttempts = 50;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                // Random position in the entire spawn area
                float x = Random.Range(spawnAreaMinX, spawnAreaMaxX);
                float z = Random.Range(spawnAreaMinZ, spawnAreaMaxZ);

                // Check if position is outside the tile area
                bool outsideTileArea = (x < tileAreaMinX || x > tileAreaMaxX || 
                                       z < tileAreaMinZ || z > tileAreaMaxZ);

                if (outsideTileArea)
                {
                    spawnPosition = new Vector3(x, 0f, z);
                    validPositionFound = true;
                }

                attempts++;
            }

            if (!validPositionFound)
            {
                // Fallback: spawn in a guaranteed safe area (far from tiles)
                int side = Random.Range(0, 4);
                float x, z;
                switch (side)
                {
                    case 0: // Top
                        x = Random.Range(0f, mapWidth);
                        z = mapHeight + minDistanceFromTiles + Random.Range(0f, spawnAreaExtensionTop - minDistanceFromTiles);
                        break;
                    case 1: // Bottom
                        x = Random.Range(0f, mapWidth);
                        z = -minDistanceFromTiles - Random.Range(0f, spawnAreaExtension - minDistanceFromTiles);
                        break;
                    case 2: // Left
                        x = -minDistanceFromTiles - Random.Range(0f, spawnAreaExtension - minDistanceFromTiles);
                        z = Random.Range(0f, mapHeight);
                        break;
                    default: // Right
                        x = mapWidth + minDistanceFromTiles + Random.Range(0f, spawnAreaExtension - minDistanceFromTiles);
                        z = Random.Range(0f, mapHeight);
                        break;
                }
                spawnPosition = new Vector3(x, 0f, z);
            }

            // Instantiate decorator
            GameObject decorator = Instantiate(prefab, spawnPosition, Quaternion.identity, decoratorParent.transform);

            // Apply random Y rotation for variety
            float randomRotation = Random.Range(0f, 360f);
            decorator.transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);

            // Apply global scale
            decorator.transform.localScale = Vector3.one * (decoratorScale + Random.Range(-0.15f, .15f));

            decorator.name = prefab.name + "_" + i;
        }

        Debug.Log($"Spawned {decoratorCount} decorators around the map");
    }

    public void ClearDecorators()
    {
        if (decoratorParent != null)
        {
            // Destroy all children
            foreach (Transform child in decoratorParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
