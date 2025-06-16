using System.Collections;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Collectable item that increases the player's maximum mana when collected.
/// </summary>
public class CollectableMaxMana : Collectable
{
=======
/// Collectable item that increases the player's maximum mana by 1 when collected.
/// </summary>
public class CollectableMaxMana : Collectable
{

    // Start is called before the first frame update.
    // Sets the shadow scale, initializes general collectable properties, and references the player's mana.
>>>>>>> Stashed changes
    void Start()
    {
        shadowScale = 1.5f;
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
    /// Increases the player's max mana when collected.
=======
    /// Called when the player collects the max mana item.
    /// Increases the player's max mana by 1 and starts the pickup animation and destruction sequence.
>>>>>>> Stashed changes
    /// </summary>
    protected override void OnCollect()
    {
        if (playerMana.GetMaxMana() < 9)
        {
            playerMana.SetMaxMana(playerMana.GetMaxMana() + 1);
        }
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
