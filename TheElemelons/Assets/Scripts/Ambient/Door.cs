using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Base class for doors that can be opened or closed by the player or game logic.
/// Handles sprite and collider state changes when interacted with.
=======
/// Represents a door that can be opened or closed by player interaction.
/// Handles sprite changes and collider toggling based on open/closed state.
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
    // Start is called before the first frame update.
    // Initializes the door's components and state.
>>>>>>> Stashed changes
    void Start()
    {
        Initialize();
    }

<<<<<<< Updated upstream
    /// <summary>
    /// Initializes the door's sprite renderer and finds the non-trigger collider.
    /// </summary>
=======
    // Finds and assigns the sprite renderer and the non-trigger box collider.
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
    /// Changes the sprite and enables/disables the collider accordingly.
>>>>>>> Stashed changes
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
