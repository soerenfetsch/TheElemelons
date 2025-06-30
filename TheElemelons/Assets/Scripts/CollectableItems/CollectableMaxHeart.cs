using System.Collections;
using UnityEngine;

/// <summary>
/// Collectable item that increases the player's maximum health by 1 and fully heals the player when collected.
/// </summary>
public class CollectableMaxHeart : Collectable
{

    // Start is called before the first frame update.
    // Sets the shadow scale, initializes general collectable properties, and references the player's health.
    void Start()
    {
        shadowScale = 1.5f;
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
    /// Called when the player collects the max heart.
    /// Increases the player's max health by 1, fully heals the player, and starts the pickup animation and destruction sequence.
    /// </summary>
    protected override void OnCollect()
    {
        if (playerHealth == null)
        {
            // When player health is not set up yet, wait for collectable to drop first
            return;
        }
        playerHealth.SetMaxHealth(playerHealth.GetMaxHealth() + 1);
        playerHealth.SetHealth(playerHealth.GetMaxHealth());
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
