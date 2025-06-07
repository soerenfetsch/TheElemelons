using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collectable item representing the earth elemelon. Triggers progression when collected and plays pickup animation.
/// </summary>
public class CollectableEarthElemelon : Collectable
{
    /// <summary>
    /// Event invoked when the earth elemelon is collected.
    /// </summary>
    public Action OnEarthElemelonCollected;

    /// <summary>
    /// Initializes general collectable references and player health reference.
    /// </summary>
    void Start()
    {
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
    }

    /// <summary>
    /// Handles collectable movement each physics frame.
    /// </summary>
    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Starts the pickup animation and triggers expiration logic when collected.
    /// </summary>
    protected override void OnCollect()
    {
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Coroutine that plays the pickup animation, disables the collider, invokes the collected event, and destroys the object.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
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

