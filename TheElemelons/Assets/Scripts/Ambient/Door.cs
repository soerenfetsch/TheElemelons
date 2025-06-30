using UnityEngine;

/// <summary>
/// Represents a door that can be opened or closed by player interaction.
/// Handles sprite changes and collider toggling based on open/closed state.
/// </summary>
public class Door : InteractableObject
{
    /// <summary>
    /// Sprite to display when the door is closed.
    /// </summary>
    public Sprite closedSprite;

    /// <summary>
    /// Sprite to display when the door is open.
    /// </summary>
    public Sprite openSprite;

    // Renderer for displaying the door's sprite.
    SpriteRenderer spriteRenderer;

    /// <summary>
    /// Indicates whether the door can currently be opened.
    /// </summary>
    public bool canOpen = true;

    // The collider used for blocking passage when the door is closed.
    BoxCollider2D boxCollider2D;

    // Start is called before the first frame update.
    // Initializes the door's components and state.
    void Start()
    {
        Initialize();
    }

    // Finds and assigns the sprite renderer and the non-trigger box collider.
    protected void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D[] boxColliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D collider2D in boxColliders)
        {
            if (collider2D.isTrigger == false)
            {
                boxCollider2D = collider2D;
            }
        }
    }

    /// <summary>
    /// Toggles the door's open/closed state if it can be opened.
    /// Changes the sprite and enables/disables the collider accordingly.
    /// </summary>
    public override void Interact()
    {
        if (canOpen)
        {
            boxCollider2D.enabled = !boxCollider2D.enabled;
            spriteRenderer.sprite = boxCollider2D.enabled ? closedSprite : openSprite;
        }
    }
}
