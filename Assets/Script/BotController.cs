using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BotController : MonoBehaviour
{
    [Header("Bot Settings")]
    public TileState myTileState = TileState.Bot1;
    public float maxMoveSpeed = 4f;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float decisionDelay = 0.5f;

    [Header("Smart Mode Settings")]
    public float smartModeSpeed = 6f; // Faster speed in smart mode

    private Vector3 currentVelocity = Vector3.zero;
    private GridManager gridManager;
    private Tile currentTile;
    private float nextDecisionTime;
    private Vector3 targetDirection;
    private bool isSmartMode = false;
    private Tile targetTile = null;

    void Start()
    {
        gridManager = GridManager.Instance;
        
        SpawnAtRandomPosition();
        CheckAndClaimTile();
        nextDecisionTime = Time.time + decisionDelay;
    }

    void SpawnAtRandomPosition()
    {
        int startX = Random.Range(0, gridManager.gridWidth);
        int startY = Random.Range(0, gridManager.gridHeight);
        
        Vector3 spawnPos = new Vector3(startX * gridManager.tileSize, 0.5f, startY * gridManager.tileSize);
        transform.position = spawnPos;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameActive() || GameManager.Instance.IsGamePaused())
            return;

        // Check if player is trapped to enable smart mode
        if (!isSmartMode && gridManager.IsPlayerTrapped())
        {
            isSmartMode = true;
            targetTile = null; // Reset target to find new one
            Debug.Log($"[{myTileState}] SMART MODE ACTIVATED! Player is trapped. Switching to aggressive pathfinding with increased speed ({smartModeSpeed}f).");
        }

        // Make decisions periodically
        if (Time.time >= nextDecisionTime)
        {
            if (isSmartMode)
            {
                MakeSmartDecision();
            }
            else
            {
                MakeDecision();
            }
            nextDecisionTime = Time.time + decisionDelay;
        }

        HandleMovement();
        CheckAndClaimTile();
    }

    void MakeDecision()
    {
        // Find empty tiles nearby
        int currentX = Mathf.RoundToInt(transform.position.x / gridManager.tileSize);
        int currentZ = Mathf.RoundToInt(transform.position.z / gridManager.tileSize);

        List<Vector3> possibleDirections = new List<Vector3>();
        Vector3[] directions = new Vector3[]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            new Vector3(1, 0, 1).normalized,
            new Vector3(-1, 0, 1).normalized,
            new Vector3(1, 0, -1).normalized,
            new Vector3(-1, 0, -1).normalized
        };

        // Check each direction for empty tiles
        foreach (Vector3 dir in directions)
        {
            Vector3 checkPos = transform.position + dir * gridManager.tileSize;
            int checkX = Mathf.RoundToInt(checkPos.x / gridManager.tileSize);
            int checkZ = Mathf.RoundToInt(checkPos.z / gridManager.tileSize);

            if (gridManager.IsValidPosition(checkX, checkZ))
            {
                Tile tile = gridManager.GetTile(checkX, checkZ);
                if (tile != null && tile.state == TileState.Empty)
                {
                    possibleDirections.Add(dir);
                }
            }
        }

        // Choose a direction
        if (possibleDirections.Count > 0)
        {
            targetDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
        }
        else
        {
            // If no empty tiles, try to move to own tiles
            foreach (Vector3 dir in directions)
            {
                Vector3 checkPos = transform.position + dir * gridManager.tileSize;
                int checkX = Mathf.RoundToInt(checkPos.x / gridManager.tileSize);
                int checkZ = Mathf.RoundToInt(checkPos.z / gridManager.tileSize);

                if (gridManager.IsValidPosition(checkX, checkZ))
                {
                    Tile tile = gridManager.GetTile(checkX, checkZ);
                    if (tile != null && tile.CanWalkOn(myTileState))
                    {
                        possibleDirections.Add(dir);
                    }
                }
            }

            if (possibleDirections.Count > 0)
            {
                targetDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
            }
            else
            {
                targetDirection = Vector3.zero;
            }
        }
    }

    void MakeSmartDecision()
    {
        int currentX = Mathf.RoundToInt(transform.position.x / gridManager.tileSize);
        int currentZ = Mathf.RoundToInt(transform.position.z / gridManager.tileSize);

        // If we don't have a target or reached it, find a new one
        if (targetTile == null || (targetTile.x == currentX && targetTile.y == currentZ))
        {
            targetTile = FindNearestReachableTile(currentX, currentZ);
            if (targetTile != null)
            {
                Debug.Log($"[{myTileState}] Smart Mode: New target found at ({targetTile.x}, {targetTile.y})");
            }
            else
            {
                Debug.Log($"[{myTileState}] Smart Mode: No reachable tiles found!");
            }
        }

        // If we have a target, move towards it
        if (targetTile != null)
        {
            Vector3 targetPos = new Vector3(targetTile.x * gridManager.tileSize, transform.position.y, targetTile.y * gridManager.tileSize);
            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0; // Keep movement horizontal
            targetDirection = direction;
        }
        else
        {
            // No reachable tiles, stop
            targetDirection = Vector3.zero;
        }
    }

    Tile FindNearestReachableTile(int startX, int startZ)
    {
        // Find all empty tiles and own unclaimed tiles
        List<Tile> candidateTiles = new List<Tile>();
        
        for (int x = 0; x < gridManager.gridWidth; x++)
        {
            for (int y = 0; y < gridManager.gridHeight; y++)
            {
                Tile tile = gridManager.GetTile(x, y);
                if (tile != null && (tile.state == TileState.Empty || tile.state == myTileState))
                {
                    candidateTiles.Add(tile);
                }
            }
        }

        // Sort by distance and check reachability
        candidateTiles = candidateTiles.OrderBy(t => 
            Mathf.Abs(t.x - startX) + Mathf.Abs(t.y - startZ)
        ).ToList();

        // Find the nearest reachable tile
        foreach (Tile tile in candidateTiles)
        {
            if (IsReachable(startX, startZ, tile.x, tile.y))
            {
                return tile;
            }
        }

        return null;
    }

    bool IsReachable(int startX, int startZ, int targetX, int targetZ)
    {
        // Simple BFS pathfinding to check if target is reachable
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(new Vector2Int(startX, startZ));
        visited.Add(new Vector2Int(startX, startZ));

        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        int maxIterations = 1000; // Prevent infinite loops
        int iterations = 0;

        while (queue.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            Vector2Int current = queue.Dequeue();

            // Check if we reached the target
            if (current.x == targetX && current.y == targetZ)
            {
                return true;
            }

            // Explore neighbors
            for (int i = 0; i < 4; i++)
            {
                int newX = current.x + dx[i];
                int newY = current.y + dy[i];
                Vector2Int neighbor = new Vector2Int(newX, newY);

                if (!visited.Contains(neighbor) && gridManager.IsValidPosition(newX, newY))
                {
                    Tile tile = gridManager.GetTile(newX, newY);
                    // Can walk on empty tiles or own tiles
                    if (tile != null && tile.CanWalkOn(myTileState))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        return false;
    }

    void HandleMovement()
    {
        // Use faster speed in smart mode
        float currentMaxSpeed = isSmartMode ? smartModeSpeed : maxMoveSpeed;
        
        // Calculate target velocity
        Vector3 targetVelocity = targetDirection * currentMaxSpeed;

        // Apply acceleration or deceleration
        if (targetDirection.magnitude > 0.1f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        // Move the bot
        if (currentVelocity.magnitude > 0.01f)
        {
            Vector3 newPosition = transform.position + currentVelocity * Time.deltaTime;
            
            if (IsPositionValid(newPosition))
            {
                transform.position = newPosition;
            }
            else
            {
                // Try sliding along walls
                Vector3 xOnlyPosition = transform.position + new Vector3(currentVelocity.x, 0, 0) * Time.deltaTime;
                if (IsPositionValid(xOnlyPosition))
                {
                    transform.position = xOnlyPosition;
                }
                else
                {
                    Vector3 zOnlyPosition = transform.position + new Vector3(0, 0, currentVelocity.z) * Time.deltaTime;
                    if (IsPositionValid(zOnlyPosition))
                    {
                        transform.position = zOnlyPosition;
                    }
                    else
                    {
                        // Stuck, stop moving
                        targetDirection = Vector3.zero;
                    }
                }
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        int gridX = Mathf.RoundToInt(position.x / gridManager.tileSize);
        int gridZ = Mathf.RoundToInt(position.z / gridManager.tileSize);

        if (!gridManager.IsValidPosition(gridX, gridZ))
        {
            return false;
        }

        Tile tile = gridManager.GetTile(gridX, gridZ);
        if (tile != null && !tile.CanWalkOn(myTileState))
        {
            return false;
        }

        return true;
    }

    void CheckAndClaimTile()
    {
        int gridX = Mathf.RoundToInt(transform.position.x / gridManager.tileSize);
        int gridZ = Mathf.RoundToInt(transform.position.z / gridManager.tileSize);

        if (gridManager.IsValidPosition(gridX, gridZ))
        {
            Tile tile = gridManager.GetTile(gridX, gridZ);
            
            if (tile != null && tile != currentTile)
            {
                if (tile.CanBeClaimedBy(myTileState))
                {
                    tile.SetState(myTileState);
                }
                currentTile = tile;
            }
        }
    }

    public void ResetPosition()
    {
        SpawnAtRandomPosition();
        currentVelocity = Vector3.zero;
        targetDirection = Vector3.zero;
        currentTile = null;
        isSmartMode = false;
        targetTile = null;
        CheckAndClaimTile();
        nextDecisionTime = Time.time + decisionDelay;
    }
}
