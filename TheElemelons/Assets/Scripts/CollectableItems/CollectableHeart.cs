using System.Collections;
using UnityEngine;

/// <summary>
/// Collectable item that heals the player by a fixed amount when collected.
/// </summary>
public class CollectableHeart : Collectable
{
    // The amount of health restored when collected.
    float healValue = 1f;

    // Start is called before the first frame update.
    // Initializes general collectable properties and references the player's health.
    void Start()
    {
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
    }

    // FixedUpdate is called once per physics frame.
    // Handles collectable movement logic.
    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Called when the player collects the heart.
    /// Heals the player and starts the pickup animation and destruction sequence.
    /// </summary>
    protected override void OnCollect()
    {
        if (playerHealth == null)
        {
            // When player health is not set up yet, wait for collectable to drop first
            return;
        }
        playerHealth.AddHealth(healValue);
        StartCoroutine(Expire());
    }

    // Plays the pickup animation, disables the collider, destroys the shadow,
    // and destroys the collectable after a short delay.
    IEnumerator Expire()
    {
        animator.SetTrigger("heal");
        collected = true;
        boxCollider2D.enabled = false;
        spriteRenderer.sortingOrder = 100;
        Destroy(shadow.gameObject);
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
