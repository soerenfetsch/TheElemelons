using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collectable item that grants the player a random buff (damage, defense, or speed) for a limited duration when collected.
/// </summary>
public class CollectablePinkPotion : Collectable
{
    /// <summary>
    /// List of possible buffs or debuffs that can be granted by the potion.
    /// </summary>
    public List<BuffDebuffManager.BuffDebuffType> buffDebuffchoices;

    /// <summary>
    /// The factor by which the buff/debuff modifies the player's stats.
    /// </summary>
    public float factor = 1.5f;

    /// <summary>
    /// Duration in seconds for which the buff/debuff is applied.
    /// </summary>
    public float duration = 120f;

    // Start is called before the first frame update.
    // Initializes general collectable properties.
    void Start()
    {
        InitializeGeneral();
    }

    // FixedUpdate is called once per physics frame.
    // Handles collectable movement logic.
    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Called when the player collects the pink potion.
    /// Applies a random buff/debuff to the player and starts the pickup animation and destruction sequence.
    /// </summary>
    protected override void OnCollect()
    {
        BuffDebuffManager.Instance.ApplyBuffDebuff(playerTransform.gameObject, "PinkPotion", ChoseBuff(), duration, factor, true);
        StartCoroutine(Expire());
    }

    // Randomly selects a buff/debuff type from the available choices.
    BuffDebuffManager.BuffDebuffType ChoseBuff()
    {
        int i = UnityEngine.Random.Range(0, buffDebuffchoices.Count);
        return buffDebuffchoices[i];
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
