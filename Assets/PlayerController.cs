using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 15f;
    public TileState myTileState = TileState.Player;

    private Vector3 currentVelocity = Vector3.zero;
    private GridManager gridManager;
    private Tile currentTile;
    private VirtualJoystick joystick;

    void Start()
    {
        gridManager = GridManager.Instance;
        joystick = VirtualJoystick.Instance;
        
        // Spawn at a random position
        int startX = Random.Range(0, gridManager.gridWidth);
        int startY = Random.Range(0, gridManager.gridHeight);
        
        Vector3 spawnPos = new Vector3(startX * gridManager.tileSize, 0.5f, startY * gridManager.tileSize);
        transform.position = spawnPos;
        
        CheckAndClaimTile();
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameActive())
            return;

        HandleMovement();
        CheckAndClaimTile();
    }

    void HandleMovement()
    {
        Vector2 input = Vector2.zero;
        
        // Get input from joystick
        if (joystick != null)
        {
            input = joystick.GetInputVector();
        }

        // Calculate target velocity
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y) * maxMoveSpeed;

        // Apply acceleration or deceleration
        if (input.magnitude > 0.1f)
        {
            // Accelerate towards target velocity
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            // Decelerate to zero
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        // Move the player
        if (currentVelocity.magnitude > 0.01f)
        {
            Vector3 newPosition = transform.position + currentVelocity * Time.deltaTime;
            
            // Check if the new position is valid (within grid bounds)
            if (IsPositionValid(newPosition))
            {
                transform.position = newPosition;
            }
            else
            {
                // Try sliding along walls
                // Try moving only in X direction
                Vector3 xOnlyPosition = transform.position + new Vector3(currentVelocity.x, 0, 0) * Time.deltaTime;
                if (IsPositionValid(xOnlyPosition))
                {
                    transform.position = xOnlyPosition;
                }
                else
                {
                    // Try moving only in Z direction
                    Vector3 zOnlyPosition = transform.position + new Vector3(0, 0, currentVelocity.z) * Time.deltaTime;
                    if (IsPositionValid(zOnlyPosition))
                    {
                        transform.position = zOnlyPosition;
                    }
                }
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        // Get grid coordinates
        int gridX = Mathf.RoundToInt(position.x / gridManager.tileSize);
        int gridZ = Mathf.RoundToInt(position.z / gridManager.tileSize);

        // Check if within bounds
        if (!gridManager.IsValidPosition(gridX, gridZ))
        {
            return false;
        }

        // Check if tile can be walked on
        Tile tile = gridManager.GetTile(gridX, gridZ);
        if (tile != null && !tile.CanWalkOn(myTileState))
        {
            return false;
        }

        return true;
    }

    void CheckAndClaimTile()
    {
        // Get current tile position
        int gridX = Mathf.RoundToInt(transform.position.x / gridManager.tileSize);
        int gridZ = Mathf.RoundToInt(transform.position.z / gridManager.tileSize);

        if (gridManager.IsValidPosition(gridX, gridZ))
        {
            Tile tile = gridManager.GetTile(gridX, gridZ);
            
            // Only claim if it's a different tile than current
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
        int startX = Random.Range(0, gridManager.gridWidth);
        int startY = Random.Range(0, gridManager.gridHeight);
        
        Vector3 spawnPos = new Vector3(startX * gridManager.tileSize, 0.5f, startY * gridManager.tileSize);
        transform.position = spawnPos;
        
        currentVelocity = Vector3.zero;
        currentTile = null;
        
        CheckAndClaimTile();
    }
}
