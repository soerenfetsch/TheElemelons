using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    // Radius within which the player can interact
    public float interactionRadius = 2f;

    // Abstract method that must be implemented by child classes
    public abstract void Interact();

    /// <summary>
    /// Returns true if the player is within interaction range
    /// </summary>
    /// <param name="player"></param>
    /// <returns>bool: true if player within interaction range</returns>
    public bool CanInteract(Transform player)
    {
        return Vector2.Distance(transform.position, player.position) <= interactionRadius;
    }
}
