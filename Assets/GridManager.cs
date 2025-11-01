using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float tileSize = 1f;
    public GameObject tilePrefab;

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
    }

    void CenterCamera()
    {
        float centerX = (gridWidth * tileSize) / 2f - tileSize / 2f;
        float centerZ = (gridHeight * tileSize) / 2f - tileSize / 2f;
        
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(centerX, 25f, centerZ);
            mainCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
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
    }
}

