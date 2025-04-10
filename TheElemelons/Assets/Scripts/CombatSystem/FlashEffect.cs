using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    /// <summary>
    /// The material to switch to during the flash.
    /// This should be a material that has a different color than the original one.
    /// </summary>
    [Tooltip("Material to switch to during the flash.")]
    public Material flashMaterial;
    /// <summary>
    /// The SpriteRenderer that should flash.
    /// </summary>
    [Tooltip("The SpriteRenderer that should flash.")]
    public SpriteRenderer spriteRenderer;
    
    // The material that was in use, when the script started.
    private Material originalMaterial;

    // The currently running coroutine.
    private Coroutine flashRoutine;

    void Start()
    {
        // Get the material that the SpriteRenderer uses, 
        // so we can switch back to it after the flash ended.
        originalMaterial = spriteRenderer?.material;
    }

    /// <summary>
    /// Starts a flash effect on the sprite renderer.
    /// Will swap the material to the flashMaterial for the duration of the effect.
    /// </summary>
    /// <param name="duration">The duration of the flash effect in seconds.</param>
    public void Flash(float duration)
    {
        if (spriteRenderer == null || flashMaterial == null)
        {
            Debug.LogWarning("FlashEffect: spriteRenderer or flashMaterial is not set. Cannot flash.");
            return;
        }
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        // Swap to the flashMaterial.
        spriteRenderer.material = flashMaterial;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        spriteRenderer.material = originalMaterial;

        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }
}
