using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Handles player interaction with nearby interactable objects in the game world.
/// Finds the closest interactable within range and triggers its interaction logic.
=======
/// Handles player interaction with nearby interactable objects.
/// Manages detection, direction, and range for interactions, and updates tooltips.
>>>>>>> Stashed changes
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    /// <summary>
    /// Reference to the ToolTip component for displaying interaction prompts.
    /// </summary>
    ToolTip toolTip;

    /// <summary>
    /// Enables or disables drawing of interaction gizmos in the editor.
    /// </summary>
    public bool gizmosEnabled = false;

    /// <summary>
    /// Layer mask used to filter interactable objects.
    /// </summary>
    public LayerMask interactableLayer;

    /// <summary>
    /// Reference to the player's Animator for determining facing direction.
    /// </summary>
    public Animator playerAnimator;

    // Offsets for the interaction box in each direction.
    Vector2 interactionBoxOffSetUp;
    Vector2 interactionBoxOffSetDown;
    Vector2 interactionBoxOffSetLeft;
    Vector2 interactionBoxOffSetRight;
    Vector2 interactionBoxOffSet;

    // Size of the interaction box for vertical and horizontal orientations.
    Vector2 interactionBoxSize;
    Vector2 interactionBoxSizeVertical;
    Vector2 interactionBoxSizeHorizontal;

<<<<<<< Updated upstream
=======
    /// <summary>
    /// Unity Start method. Initializes interaction box offsets and gets the ToolTip component.
    /// </summary>
>>>>>>> Stashed changes
    void Start()
    {
        InitializeInteractionBoxes();
        toolTip = GetComponent<ToolTip>();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Initializes the offsets and sizes for the interaction boxes in each direction.
=======
    /// Initializes the offsets and sizes for the interaction box in all directions.
>>>>>>> Stashed changes
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
    /// Finds the closest interactable object to the player within interaction range
    /// and interacts with it. If no interactable object is found, nothing happens.
    /// </summary>
    public void InteractWithClosestInteractable()
    {
        InteractableObject closestInteractable = FindClosestInteractable();
        if (closestInteractable != null)
        {
            closestInteractable.Interact();
        }
    }

<<<<<<< Updated upstream
    /// <summary>
    /// Calculates the direction offset for the interaction box based on player movement.
    /// </summary>
    /// <returns>The offset vector for the interaction box.</returns>
=======
    // Calculates the direction offset for the interaction box based on player facing direction.
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    /// <summary>
    /// Calculates the size of the interaction box based on the current direction.
    /// </summary>
    /// <returns>The size vector for the interaction box.</returns>
=======
    // Calculates the size of the interaction box based on its orientation.
>>>>>>> Stashed changes
    Vector2 CalcSize()
    {
        interactionBoxSize = interactionBoxSizeHorizontal;
        if (interactionBoxOffSet == interactionBoxOffSetDown || interactionBoxOffSet == interactionBoxOffSetUp)
        {
            interactionBoxSize = interactionBoxSizeVertical;
        }
        return interactionBoxSize;
    }

<<<<<<< Updated upstream
=======
    /// <summary>
    /// Draws the interaction box gizmo in the editor for debugging.
    /// </summary>
>>>>>>> Stashed changes
    void OnDrawGizmos()
    {
        if (gizmosEnabled)
        {
            Gizmos.DrawCube(transform.position + (Vector3)CalcDir(), size: CalcSize());
        }
    }

    /// <summary>
    /// Finds the closest interactable object to the player within interaction range.
    /// </summary>
    /// <returns>The closest <see cref="InteractableObject"/> or null if none found.</returns>
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

<<<<<<< Updated upstream
=======
    /// <summary>
    /// Unity Update method. Continuously checks for the closest interactable and updates the tooltip.
    /// </summary>
>>>>>>> Stashed changes
    void Update()
    {
        // Continuously update the tooltip based on whether an interactable is in range.
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
