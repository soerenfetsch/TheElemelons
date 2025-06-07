using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for doors that can be opened or closed by the player or game logic.
/// Handles sprite and collider state changes when interacted with.
/// </summary>
public class Door : InteractableObject
{
    public Sprite closedSprite;
    public Sprite openSprite;
    SpriteRenderer spriteRenderer;
    public bool canOpen = true;
    BoxCollider2D boxCollider2D;

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the door's sprite renderer and finds the non-trigger collider.
    /// </summary>
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
