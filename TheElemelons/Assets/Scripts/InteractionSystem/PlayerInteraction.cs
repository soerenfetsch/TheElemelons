using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // Radius within which the player can interact
    public float interactionRange = 2f;
    public LayerMask interactableLayer; // Ensure interactable objects have this layer

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
    /// Finds the closest interactable object to the player within interactionRange
    /// of the player which can be interacted with.
    /// </summary>
    /// <returns>InteractableObject or null if none found</returns>
    private InteractableObject FindClosestInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
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
}
