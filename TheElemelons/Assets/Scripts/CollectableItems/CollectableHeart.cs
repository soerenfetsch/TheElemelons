using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collectable item that restores health to the player when collected.
/// </summary>
public class CollectableHeart : Collectable
{
    float healValue = 1f;

    void Start()
    {
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Restores health to the player and starts the pickup animation when collected.
    /// </summary>
    protected override void OnCollect()
    {
        playerHealth.AddHealth(healValue);
        StartCoroutine(Expire());
    }

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
