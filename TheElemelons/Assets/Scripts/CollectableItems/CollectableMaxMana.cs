using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collectable item that increases the player's maximum mana when collected.
/// </summary>
public class CollectableMaxMana : Collectable
{
    void Start()
    {
        shadowScale = 1.5f;
        InitializeGeneral();
        playerMana = player.GetComponent<Mana>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Increases the player's max mana when collected.
    /// </summary>
    protected override void OnCollect()
    {
        playerMana.SetMaxMana(playerMana.GetMaxMana() + 1);
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
