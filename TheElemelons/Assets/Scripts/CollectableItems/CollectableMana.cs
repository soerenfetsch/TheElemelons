using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collectable item that restores mana to the player when collected.
/// </summary>
public class CollectableMana : Collectable
{
    void Start()
    {
        InitializeGeneral();
        playerMana = player.GetComponent<Mana>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Restores mana to the player and starts the pickup animation when collected.
    /// </summary>
    protected override void OnCollect()
    {
        playerMana.AddMana(1);
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
