using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Handles the transparency of the King's Bed roof sprite when the player moves behind or away from it.
=======
/// Handles the visual effect of the King's Bed roof becoming transparent when the player is behind it.
/// Adjusts the alpha of the roof sprite based on player trigger events.
>>>>>>> Stashed changes
/// </summary>
public class KingsBed : MonoBehaviour
{
    /// <summary>
    /// The alpha value to use for the roof sprite when the player is behind the bed.
    /// </summary>
    public float opacityPlayerBehind = 0.35f;

    // Reference to the SpriteRenderer for the roof.
    SpriteRenderer spriteRendererRoof;

    /// <summary>
    /// Start is called before the first frame update.
    /// This method initializes the spriteRendererRoof by finding the "KingsBed/Roof" GameObject
    /// and setting its alpha to 1.0f.
    /// </summary>
    void Start()
    {
        spriteRendererRoof = GameObject.Find("KingsBed/Roof").GetComponent<SpriteRenderer>();
        SetAlpha(spriteRendererRoof, 1.0f);
    }

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object.
    /// If the collider belongs to the player and is a trigger, it sets the alpha of the roof sprite to a lower value.
    /// </summary>
    /// <param name="other">The Collider2D that has entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.isTrigger)
        {
            SetAlpha(spriteRendererRoof, opacityPlayerBehind);
        }
    }

    /// <summary>
    /// Called when another collider exits the trigger collider attached to this object.
    /// If the collider belongs to the player and is a trigger, it sets the alpha of the roof sprite back to 1.0f.
    /// </summary>
    /// <param name="other">The Collider2D that has exited the trigger.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.isTrigger)
        {
            SetAlpha(spriteRendererRoof, 1.0f);
        }
    }

    /// <summary>
    /// Sets the alpha value of the given sprite renderer.
    /// Alpha should be between 0.0f and 1.0f.
    /// </summary>
    /// <param name="spriteRenderer">The SpriteRenderer to modify.</param>
    /// <param name="alpha">The alpha value to set.</param>
    public void SetAlpha(SpriteRenderer spriteRenderer, float alpha)
    {
        if (0.0f > alpha || alpha > 1.0f)
        {
            Debug.LogError("Alpha value must be between 0.0f and 1.0f.");
            return;
        }
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}