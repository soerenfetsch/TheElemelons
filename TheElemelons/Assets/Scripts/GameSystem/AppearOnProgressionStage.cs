using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enables or disables GameObjects and their components based on the current game progression stage.
/// Useful for controlling the visibility and interactivity of objects as the player advances through the story.
/// </summary>
public class AppearOnProgressionStage : MonoBehaviour
{
    /// <summary>
    /// List of progression stages during which this object should be active.
    /// </summary>
    public List<GameProgressionStage> allowedGameProgressionStages;

    // Start is called before the first frame update.
    // Sets the initial active state based on the current progression stage.
    void Start()
    {
        bool shouldBeActive = allowedGameProgressionStages != null && allowedGameProgressionStages.Contains(GameManager.Instance.CurrentProgressionStage);
        SetObjectsActive(shouldBeActive);
    }

    // Updates visibility based on the current progression stage.
    // Only checks every 60 frames (about once per second) to avoid performance issues.
    void Update()
    {
        if (Time.frameCount % 60 == 0) // Check every second
        {
            bool shouldBeActive = allowedGameProgressionStages != null && allowedGameProgressionStages.Contains(GameManager.Instance.CurrentProgressionStage);
            SetObjectsActive(shouldBeActive);
        }
    }

    // Enables or disables all Collider2D, SpriteRenderer, Rigidbody2D components,
    // and child GameObjects based on the 'active' parameter.
    private void SetObjectsActive(bool active)
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = active;
        }

        SpriteRenderer[] renderers = GetComponents<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = active;
        }

        Rigidbody2D[] rigidbodies = GetComponents<Rigidbody2D>();
        foreach (Rigidbody2D rb in rigidbodies)
        {
            rb.simulated = active;
        }

        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(active);
            }
        }
    }
}
