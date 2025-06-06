using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearOnProgressionStage : MonoBehaviour
{
    public List<GameProgressionStage> allowedGameProgressionStages;
    // Start is called before the first frame update
    void Start()
    {
        bool shouldBeActive = allowedGameProgressionStages != null && allowedGameProgressionStages.Contains(GameManager.Instance.CurrentProgressionStage);
        SetObjectsActive(shouldBeActive);
    }

    void Update()
    {
        // Update visibility based on the current progression stage
        // Only update every coupe of seconds to avoid performance issues
        if (Time.frameCount % 60 == 0) // Check every second
        {
            bool shouldBeActive = allowedGameProgressionStages != null && allowedGameProgressionStages.Contains(GameManager.Instance.CurrentProgressionStage);
            SetObjectsActive(shouldBeActive);
        }
    }

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
