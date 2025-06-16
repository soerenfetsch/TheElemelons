using System.Collections.Generic;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Enables or disables GameObjects and their components based on the current game progression stage.
/// Useful for controlling the visibility and interactivity of objects as the player advances through the story.
=======
/// Controls the visibility and interactivity of a GameObject and its children
/// based on the current <see cref="GameProgressionStage"/>.
/// Enables or disables colliders, renderers, rigidbodies, and child GameObjects
/// depending on whether the current stage is in <see cref="allowedGameProgressionStages"/>.
>>>>>>> Stashed changes
/// </summary>
public class AppearOnProgressionStage : MonoBehaviour
{
    /// <summary>
<<<<<<< Updated upstream
    /// List of progression stages during which this object should be active.
    /// </summary>
    public List<GameProgressionStage> allowedGameProgressionStages;

    // Start is called before the first frame update.
    // Sets the initial active state based on the current progression stage.
=======
    /// List of allowed progression stages in which this object should be active.
    /// </summary>
    public List<GameProgressionStage> allowedGameProgressionStages;

    /// <summary>
    /// Unity Start method. Sets the object's active state based on the current progression stage.
    /// </summary>
>>>>>>> Stashed changes
    void Start()
    {
        bool shouldBeActive = allowedGameProgressionStages != null && allowedGameProgressionStages.Contains(GameManager.Instance.CurrentProgressionStage);
        SetObjectsActive(shouldBeActive);
    }

<<<<<<< Updated upstream
    // Updates visibility based on the current progression stage.
    // Only checks every 60 frames (about once per second) to avoid performance issues.
    void Update()
    {
=======
    /// <summary>
    /// Unity Update method. Periodically updates the object's active state based on the current progression stage.
    /// </summary>
    void Update()
    {
        // Update visibility based on the current progression stage
        // Only update every couple of seconds to avoid performance issues
>>>>>>> Stashed changes
        if (Time.frameCount % 60 == 0) // Check every second
        {
            bool shouldBeActive = allowedGameProgressionStages != null && allowedGameProgressionStages.Contains(GameManager.Instance.CurrentProgressionStage);
            SetObjectsActive(shouldBeActive);
        }
    }

<<<<<<< Updated upstream
    // Enables or disables all Collider2D, SpriteRenderer, Rigidbody2D components,
    // and child GameObjects based on the 'active' parameter.
=======
    /// <summary>
    /// Enables or disables all relevant components and child GameObjects based on the active flag.
    /// </summary>
    /// <param name="active">If true, enables components and children; otherwise disables them.</param>
>>>>>>> Stashed changes
    private void SetObjectsActive(bool active)
    {
        // Enable/disable all Collider2D components
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = active;
        }

        // Enable/disable all SpriteRenderer components
        SpriteRenderer[] renderers = GetComponents<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = active;
        }

        // Enable/disable all Rigidbody2D components (by toggling simulation)
        Rigidbody2D[] rigidbodies = GetComponents<Rigidbody2D>();
        foreach (Rigidbody2D rb in rigidbodies)
        {
            rb.simulated = active;
        }

        // Enable/disable all child GameObjects
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }
        }
    }
}
