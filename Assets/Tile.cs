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
    
    private Color emptyColor = new Color(0.7f, 0.7f, 0.7f); // Gray
    private Color playerColor = Color.blue;
    private Color bot1Color = Color.red;
    private Color bot2Color = Color.yellow;
    private Color bot3Color = Color.green;

    void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        tileMaterial = tileRenderer.material;
    }

    public void Initialize(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
        SetState(TileState.Empty);
    }

    public void SetState(TileState newState)
    {
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
