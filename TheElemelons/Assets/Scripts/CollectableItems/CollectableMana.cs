using System.Collections;
using UnityEngine;

/// <summary>
/// Collectable item that restores mana to the player when collected.
/// </summary>
public class CollectableMana : Collectable
{
<<<<<<< Updated upstream
=======

    // Start is called before the first frame update.
    // Initializes general collectable properties and references the player's mana.
>>>>>>> Stashed changes
    void Start()
    {
        InitializeGeneral();
        playerMana = player.GetComponent<Mana>();
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
    /// Restores mana to the player and starts the pickup animation when collected.
=======
    /// Called when the player collects the mana item.
    /// Restores 1 mana to the player and starts the pickup animation and destruction sequence.
>>>>>>> Stashed changes
    /// </summary>
    protected override void OnCollect()
    {
        playerMana.AddMana(1);
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
