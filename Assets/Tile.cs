using UnityEngine;

public enum TileState
{
    Empty,
    Player,
    Bot1,
    Bot2,
    Bot3
}

public class Tile : MonoBehaviour
{
    public TileState state = TileState.Empty;
    public int x, y;
    private Renderer tileRenderer;
    private Material tileMaterial;
    
    public Color emptyColor = new Color(0.7f, 0.7f, 0.7f); // Gray
    public Color playerColor = Color.blue;
    public Color bot1Color = Color.red;
    public Color bot2Color = Color.yellow;
    public Color bot3Color = Color.green;

    void Awake()
    {
        // Get renderer from child (TileModel)
        tileRenderer = GetComponentInChildren<Renderer>();
        
        if (tileRenderer != null)
        {
            // The FBX has 2 materials, we want to modify the second one (index 1)
            if (tileRenderer.materials.Length > 1)
            {
                // Create a copy of the materials array
                Material[] materials = tileRenderer.materials;
                // Get the second material (index 1)
                tileMaterial = materials[1];
            }
            else
            {
                // Fallback to first material if only one exists
                tileMaterial = tileRenderer.material;
            }
        }
    }

    public void Initialize(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
        SetState(TileState.Empty);
    }

    public void SetState(TileState newState)
    {
        // Play claim tile sound only when PLAYER claims a tile
        if (newState == TileState.Player && state != TileState.Player)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayClaimTile();
            }
        }

        state = newState;
        UpdateColor();
    }

    private void UpdateColor()
    {
        switch (state)
        {
            case TileState.Empty:
                tileMaterial.color = emptyColor;
                break;
            case TileState.Player:
                tileMaterial.color = playerColor;
                break;
            case TileState.Bot1:
                tileMaterial.color = bot1Color;
                break;
            case TileState.Bot2:
                tileMaterial.color = bot2Color;
                break;
            case TileState.Bot3:
                tileMaterial.color = bot3Color;
                break;
        }
    }

    public bool CanBeClaimedBy(TileState claimant)
    {
        // Can claim if empty or already owned by the same entity
        return state == TileState.Empty || state == claimant;
    }

    public bool CanWalkOn(TileState walker)
    {
        // Can walk on empty tiles or own tiles
        return state == TileState.Empty || state == walker;
    }
}
