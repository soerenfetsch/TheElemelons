using System.Collections;
using UnityEngine;

/// <summary>
/// Collectable item that restores mana to the player when collected.
/// </summary>
public class CollectableMana : Collectable
{

    // Start is called before the first frame update.
    // Initializes general collectable properties and references the player's mana.
    void Start()
    {
        InitializeGeneral();
        playerMana = player.GetComponent<Mana>();
    }

    // FixedUpdate is called once per physics frame.
    // Handles collectable movement logic.
    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Called when the player collects the mana item.
    /// Restores 1 mana to the player and starts the pickup animation and destruction sequence.
    /// </summary>
    protected override void OnCollect()
    {
        playerMana.AddMana(1);
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
