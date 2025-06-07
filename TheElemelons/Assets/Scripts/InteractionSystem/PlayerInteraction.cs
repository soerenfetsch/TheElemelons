using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles player interaction with nearby interactable objects in the game world.
/// Finds the closest interactable within range and triggers its interaction logic.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    ToolTip toolTip;
    // Radius within which the player can interact
    public bool gizmosEnabled = false;
    public LayerMask interactableLayer; // Ensure interactable objects have this layer
    public Animator playerAnimator;
    Vector2 interactionBoxOffSetUp;
    Vector2 interactionBoxOffSetDown;
    Vector2 interactionBoxOffSetLeft;
    Vector2 interactionBoxOffSetRight;
    Vector2 interactionBoxOffSet;
    Vector2 interactionBoxSize;
    Vector2 interactionBoxSizeVertical;
    Vector2 interactionBoxSizeHorizontal;

    void Start()
    {
        InitializeInteractionBoxes();
        toolTip = GetComponent<ToolTip>();
    }

    /// <summary>
    /// Initializes the offsets and sizes for the interaction boxes in each direction.
    /// </summary>
    public void InitializeInteractionBoxes()
    {
        interactionBoxOffSetDown = new Vector2(-0.076f, -0.245f);
        interactionBoxOffSetUp = new Vector2(0.05f, 1);
        interactionBoxOffSetRight = new Vector2(0.770f, 0.392f);
        interactionBoxOffSetLeft = new Vector2(-0.770f, 0.392f);
        interactionBoxSizeVertical = new Vector2(0.63f, 1.25f);
        interactionBoxSizeHorizontal = new Vector2(1f, 0.63f);
    }

    /// <summary>
    /// Finds the closest interactable object to the player within interactionRange
    /// of the player which can be interacted with and interacts with it.
    /// If no interactable object is found, nothing happens.
    /// </summary>
    public void InteractWithClosestInteractable()
    {
        InteractableObject closestInteractable = FindClosestInteractable();
        if (closestInteractable != null)
        {
            closestInteractable.Interact();
        }
    }

    /// <summary>
    /// Calculates the direction offset for the interaction box based on player movement.
    /// </summary>
    /// <returns>The offset vector for the interaction box.</returns>
    Vector2 CalcDir()
    {
        Vector2 movement = new Vector2(playerAnimator.GetFloat("Horizontal"), playerAnimator.GetFloat("Vertical"));
        interactionBoxOffSet = interactionBoxOffSetRight;
        if (Mathf.Abs(movement.x) < Mathf.Abs(movement.y))
        {
            interactionBoxOffSet = interactionBoxOffSetUp;
            if (movement.y < 0)
            {
                interactionBoxOffSet = interactionBoxOffSetDown;
            }
        }
        else if (movement.x < 0)
        {
            interactionBoxOffSet = interactionBoxOffSetLeft;
        }
        return interactionBoxOffSet;
    }

    /// <summary>
    /// Calculates the size of the interaction box based on the current direction.
    /// </summary>
    /// <returns>The size vector for the interaction box.</returns>
    Vector2 CalcSize()
    {
        interactionBoxSize = interactionBoxSizeHorizontal;
        if (interactionBoxOffSet == interactionBoxOffSetDown || interactionBoxOffSet == interactionBoxOffSetUp)
        {
            interactionBoxSize = interactionBoxSizeVertical;
        }
        return interactionBoxSize;
    }

    void OnDrawGizmos()
    {
        if (gizmosEnabled)
        {
            Gizmos.DrawCube(transform.position + (Vector3)CalcDir(), size: CalcSize());
        }
    }

    /// <summary>
    /// Finds the closest interactable object to the player within interactionRange
    /// of the player which can be interacted with.
    /// </summary>
    /// <returns>InteractableObject or null if none found</returns>
    private InteractableObject FindClosestInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)CalcDir(),
        size: CalcSize(), angle: 0, layerMask: interactableLayer);
        InteractableObject closestInteractable = null;
        float minDistance = Mathf.Infinity;
        foreach (Collider2D collider in colliders)
        {
            InteractableObject interactable = collider.GetComponent<InteractableObject>();
            if (interactable != null && interactable.CanInteract(transform))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }
        return closestInteractable;
    }

    void Update()
    {
        if (true)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)CalcDir(),
        size: CalcSize(), angle: 0, layerMask: interactableLayer);
            InteractableObject closestInteractable = null;
            float minDistance = Mathf.Infinity;
            foreach (Collider2D collider in colliders)
            {
                InteractableObject interactable = collider.GetComponent<InteractableObject>();
                if (interactable != null && interactable.CanInteract(transform))
                {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }
            toolTip.canInteract = closestInteractable != null;
        }
    }
}
