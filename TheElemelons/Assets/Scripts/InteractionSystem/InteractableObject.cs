using UnityEngine;

/// <summary>
/// Abstract base class for all interactable objects in the game.
/// Provides an interface for interaction logic and proximity checks.
/// </summary>
public abstract class InteractableObject : MonoBehaviour
{
    /// <summary>
    /// The radius within which the player can interact with this object.
    /// </summary>
    public float interactionRadius = 2f;

    /// <summary>
    /// Abstract method that must be implemented by child classes to define interaction behavior.
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// Checks if the player is within interaction range of this object.
    /// </summary>
    /// <param name="player">The player's transform.</param>
    /// <returns>True if the player is within <see cref="interactionRadius"/>; otherwise, false.</returns>
    public virtual bool CanInteract(Transform player)
    {
        return Vector2.Distance(transform.position, player.position) <= interactionRadius;
    }
}
