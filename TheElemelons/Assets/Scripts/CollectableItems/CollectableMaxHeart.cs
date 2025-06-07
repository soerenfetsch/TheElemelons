using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collectable item that increases the player's maximum health when collected.
/// </summary>
public class CollectableMaxHeart : Collectable
{
    void Start()
    {
        shadowScale = 1.5f;
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Increases the player's max health and fully heals them when collected.
    /// </summary>
    protected override void OnCollect()
    {
        playerHealth.SetMaxHealth(playerHealth.GetMaxHealth() + 1);
        playerHealth.SetHealth(playerHealth.GetMaxHealth());
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
