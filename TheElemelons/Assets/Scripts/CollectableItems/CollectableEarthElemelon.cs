using System;
using System.Collections;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Collectable item representing the earth elemelon. Triggers progression when collected and plays pickup animation.
=======
/// Special collectable item representing the Earth Elemelon.
/// When collected, triggers a progression event and plays a pickup animation.
>>>>>>> Stashed changes
/// </summary>
public class CollectableEarthElemelon : Collectable
{
    /// <summary>
<<<<<<< Updated upstream
    /// Event invoked when the earth elemelon is collected.
    /// </summary>
    public Action OnEarthElemelonCollected;

    /// <summary>
    /// Initializes general collectable references and player health reference.
    /// </summary>
=======
    /// Event invoked when the earth elemelon is collected by the player.
    /// Used to trigger progression in the main game.
    /// </summary>
    public Action OnEarthElemelonCollected;

    // Start is called before the first frame update.
    // Initializes general collectable properties and references the player's health.
>>>>>>> Stashed changes
    void Start()
    {
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
    }

<<<<<<< Updated upstream
    /// <summary>
    /// Handles collectable movement each physics frame.
    /// </summary>
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
    /// Starts the pickup animation and triggers expiration logic when collected.
=======
    /// Called when the player collects the earth elemelon.
    /// Starts the pickup animation and triggers the collection event after a delay.
>>>>>>> Stashed changes
    /// </summary>
    protected override void OnCollect()
    {
        StartCoroutine(Expire());
    }

<<<<<<< Updated upstream
    /// <summary>
    /// Coroutine that plays the pickup animation, disables the collider, invokes the collected event, and destroys the object.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
=======
    // Plays the pickup animation, disables the collider, destroys the shadow,
    // invokes the collection event, and destroys the collectable after a short delay.
>>>>>>> Stashed changes
    IEnumerator Expire()
    {
        animator.SetTrigger("pickUp");
        collected = true;
        boxCollider2D.enabled = false;
        spriteRenderer.sortingOrder = 100;
        Destroy(shadow.gameObject);
        yield return new WaitForSeconds(0.6f);
        OnEarthElemelonCollected?.Invoke();
        Destroy(gameObject);
    }
}

