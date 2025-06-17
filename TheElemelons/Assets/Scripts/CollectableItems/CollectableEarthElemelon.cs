using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Special collectable item representing the Earth Elemelon.
/// When collected, triggers a progression event and plays a pickup animation.
/// </summary>
public class CollectableEarthElemelon : Collectable
{
    /// <summary>
    /// Event invoked when the earth elemelon is collected by the player.
    /// Used to trigger progression in the main game.
    /// </summary>
    public Action OnEarthElemelonCollected;

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
    /// Called when the player collects the earth elemelon.
    /// Starts the pickup animation and triggers the collection event after a delay.
    /// </summary>
    protected override void OnCollect()
    {
        StartCoroutine(Expire());
    }

    // Plays the pickup animation, disables the collider, destroys the shadow,
    // invokes the collection event, and destroys the collectable after a short delay.
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

