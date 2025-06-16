using System.Collections;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Collectable item that restores health to the player when collected.
=======
/// Collectable item that heals the player by a fixed amount when collected.
>>>>>>> Stashed changes
/// </summary>
public class CollectableHeart : Collectable
{
    // The amount of health restored when collected.
    float healValue = 1f;

<<<<<<< Updated upstream
=======
    // Start is called before the first frame update.
    // Initializes general collectable properties and references the player's health.
>>>>>>> Stashed changes
    void Start()
    {
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
    }

<<<<<<< Updated upstream
=======
    // FixedUpdate is called once per physics frame.
    // Handles collectable movement logic.
>>>>>>> Stashed changes
    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Restores health to the player and starts the pickup animation when collected.
=======
    /// Called when the player collects the heart.
    /// Heals the player and starts the pickup animation and destruction sequence.
>>>>>>> Stashed changes
    /// </summary>
    protected override void OnCollect()
    {
        playerHealth.AddHealth(healValue);
        StartCoroutine(Expire());
    }

<<<<<<< Updated upstream
=======
    // Plays the pickup animation, disables the collider, destroys the shadow,
    // and destroys the collectable after a short delay.
>>>>>>> Stashed changes
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
