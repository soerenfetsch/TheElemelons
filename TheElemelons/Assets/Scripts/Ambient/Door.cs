using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public override void Interact()
    {
        if (canOpen)
        {
            boxCollider2D.enabled = !boxCollider2D.enabled;
            spriteRenderer.sprite = boxCollider2D.enabled ? closedSprite : openSprite;
        }
    }
}
