using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float tileSize = 1f;
    public GameObject tilePrefab;

    [Header("Camera Settings")]
    [Tooltip("Camera X rotation angle (0 = horizontal, 90 = top-down)")]
    public float cameraAngle = 60f;
    [Tooltip("Distance multiplier for camera positioning")]
    public float cameraDistanceMultiplier = 1.5f;
    [Tooltip("Additional height offset for camera")]
    public float cameraHeightOffset = 5f;
    [Tooltip("Base field of view for 16:9 aspect ratio")]
    public float baseFOV = 60f;
    [Tooltip("Reference aspect ratio (width/height). Default is 16:9 = 0.5625")]
    public float referenceAspectRatio = 0.5625f; // 9/16 for portrait

    [Header("Decorators")]
    public DecoratorManager decoratorManager;

    private Tile[,] grid;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Get map size from GameSettings if available
        GameSettings settings = FindObjectOfType<GameSettings>();
        if (settings != null)
        {
            int size = settings.GetMapSize();
            gridWidth = size;
            gridHeight = size;
        }
        
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new Tile[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileObj.name = $"Tile_{x}_{y}";

                Tile tile = tileObj.GetComponent<Tile>();
                tile.Initialize(x, y);
                grid[x, y] = tile;
            }
        }

        // Center camera on grid
        CenterCamera();

        // Spawn decorators around the map
        if (decoratorManager != null)
        {
            //decoratorManager.SpawnDecorators(gridWidth, gridHeight, tileSize);
        }
    }

    void CenterCamera()
    {
        float centerX = (gridWidth * tileSize) / 2f - tileSize / 2f;
        float centerZ = (gridHeight * tileSize) / 2f - tileSize / 2f;
        
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Switch to perspective camera
            mainCamera.orthographic = false;
            
            // Calculate current aspect ratio (width/height)
            float currentAspectRatio = (float)Screen.width / (float)Screen.height;
            
            // Adjust FOV based on aspect ratio
            // For narrower screens (taller), increase FOV to show full map width
            float adjustedFOV = baseFOV;
            if (currentAspectRatio < referenceAspectRatio)
            {
                // Screen is narrower than reference, increase FOV
                float aspectRatioDifference = referenceAspectRatio / currentAspectRatio;
                adjustedFOV = baseFOV * aspectRatioDifference;
                // Clamp FOV to reasonable values
                adjustedFOV = Mathf.Clamp(adjustedFOV, baseFOV, 120f);
            }
            
            mainCamera.fieldOfView = adjustedFOV;
            
            // Calculate camera distance based on map size
            float mapDiagonal = Mathf.Sqrt(gridWidth * gridWidth + gridHeight * gridHeight) * tileSize;
            float cameraDistance = mapDiagonal * cameraDistanceMultiplier;
            
            // Calculate camera position based on angle
            float angleInRadians = cameraAngle * Mathf.Deg2Rad;
            float height = Mathf.Sin(angleInRadians) * cameraDistance + cameraHeightOffset;
            float horizontalDistance = Mathf.Cos(angleInRadians) * cameraDistance;
            
            // Position camera behind and above the center
            Vector3 cameraPosition = new Vector3(centerX, height, centerZ - horizontalDistance);
            mainCamera.transform.position = cameraPosition;
            
            // Look at the center of the grid
            mainCamera.transform.LookAt(new Vector3(centerX, 0, centerZ));
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            return grid[x, y];
        }
        return null;
    }

    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    public int CountTilesByState(TileState state)
    {
        int count = 0;
        foreach (Tile tile in grid)
        {
            if (tile.state == state)
            {
                count++;
            }
        }
        return count;
    }

    public void ResetGrid()
    {
        foreach (Tile tile in grid)
        {
            tile.SetState(TileState.Empty);
        }

        // Respawn decorators when resetting the grid
        if (decoratorManager != null)
        {
            decoratorManager.SpawnDecorators(gridWidth, gridHeight, tileSize);
        }
    }

    // Check if the player is trapped (cannot expand their territory)
    public bool IsPlayerTrapped()
    {
        // Find all player tiles
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Tile tile = grid[x, y];
                if (tile.state == TileState.Player)
                {
                    // Check all 4 adjacent tiles (up, down, left, right)
                    int[] dx = { 0, 0, -1, 1 };
                    int[] dy = { -1, 1, 0, 0 };

                    for (int i = 0; i < 4; i++)
                    {
                        int newX = x + dx[i];
                        int newY = y + dy[i];

                        if (IsValidPosition(newX, newY))
                        {
                            Tile adjacentTile = grid[newX, newY];
                            // If there's an empty tile or player's own tile adjacent, player is NOT trapped
                            if (adjacentTile.state == TileState.Empty || adjacentTile.state == TileState.Player)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        // If we get here, all player tiles are surrounded by bot tiles
        return true;
    }

    // Get all tiles of a specific state
    public List<Tile> GetTilesByState(TileState state)
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Tile tile in grid)
        {
            if (tile.state == state)
            {
                tiles.Add(tile);
            }
        }
        return tiles;
    }
}

