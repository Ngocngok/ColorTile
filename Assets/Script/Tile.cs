using UnityEngine;
using System.Collections;

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

    [Header("Animation Settings")]
    public float animationDuration = 0.3f;
    public float scaleDownAmount = 0.7f;
    
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

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

        // Store the original scale
        originalScale = transform.localScale;
    }

    public void Initialize(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
        SetState(TileState.Empty);
    }

    public void SetState(TileState newState)
    {
        // Only play animation and sound when tile is being claimed (changing from Empty to a claimed state)
        bool isClaiming = state == TileState.Empty && newState != TileState.Empty;
        
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

        // Play spring animation when tile is claimed
        if (isClaiming)
        {
            PlayClaimAnimation();
        }
    }

    private void PlayClaimAnimation()
    {
        // Stop any existing animation
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(SpringScaleAnimation());
    }

    private IEnumerator SpringScaleAnimation()
    {
        float elapsed = 0f;
        float halfDuration = animationDuration / 2f;

        // Scale down phase (only X and Z axes for width and length)
        Vector3 targetScaleDown = new Vector3(
            originalScale.x * scaleDownAmount,
            originalScale.y, // Keep Y (height) unchanged
            originalScale.z * scaleDownAmount
        );

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            // Ease out for smooth deceleration
            t = 1f - Mathf.Pow(1f - t, 2f);
            transform.localScale = Vector3.Lerp(originalScale, targetScaleDown, t);
            yield return null;
        }

        // Scale back up phase with spring effect
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            // Ease out elastic for spring effect
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.localScale = Vector3.Lerp(targetScaleDown, originalScale, t);
            yield return null;
        }

        // Ensure we end at exactly the original scale
        transform.localScale = originalScale;
        scaleCoroutine = null;
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
