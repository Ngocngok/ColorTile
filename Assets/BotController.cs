using UnityEngine;
using System.Collections.Generic;

public class BotController : MonoBehaviour
{
    [Header("Bot Settings")]
    public TileState myTileState = TileState.Bot1;
    public float maxMoveSpeed = 4f;
    public float acceleration = 8f;
    public float deceleration = 12f;
    public float decisionDelay = 0.5f;

    private Vector3 currentVelocity = Vector3.zero;
    private GridManager gridManager;
    private Tile currentTile;
    private float nextDecisionTime;
    private Vector3 targetDirection;

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
        if (!GameManager.Instance.IsGameActive())
            return;

        // Make decisions periodically
        if (Time.time >= nextDecisionTime)
        {
            MakeDecision();
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

    void HandleMovement()
    {
        // Calculate target velocity
        Vector3 targetVelocity = targetDirection * maxMoveSpeed;

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
        CheckAndClaimTile();
        nextDecisionTime = Time.time + decisionDelay;
    }
}
