using System.Collections;
using UnityEngine;

/// <summary>
/// Spell to cleanse the player of all buffs and debuffs.
/// Instantly removes all active effects from the caster, plays an animation, and disables input during the animation.
/// </summary>
public class CleanseSpell : SpellEffect
{
    // Reference to the caster's transform.
    Transform casterTransform;
    // Duration of the cleanse animation.
    float animationtime = 2.3f;
    // Reference to the caster's InputManager for disabling input.
    InputManager playerInput;
    // Animator for the cleanse animation.
    Animator animator;
    // Reference to the caster's PlayerMovement for animation direction.
    PlayerMovement playerMovement;

    /// <summary>
    /// Unity Start method. Initializes references, removes buffs, and starts the animation/expire coroutine.
    /// </summary>
    void Start()
    {
        casterTransform = caster.transform;
        animator = GetComponent<Animator>();
        try
        {
            playerInput = caster.GetComponent<InputManager>();
            playerMovement = caster.GetComponent<PlayerMovement>();
        }
        catch (System.Exception)
        {
            throw;
        }
        RemoveBuffs();
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Unity FixedUpdate method. Follows the caster and updates animation direction.
    /// </summary>
    void FixedUpdate()
    {
        // Keep the spell effect slightly below the caster and update animation direction.
        transform.position = casterTransform.position + (Vector3)Vector2.down * 0.001f;
        Vector2 movement = playerMovement.GetMovement();
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }

    /// <summary>
    /// Disables input, waits for the animation to finish, then re-enables input and destroys the spell object.
    /// </summary>
    IEnumerator Expire()
    {
        if (playerInput != null)
        {
            playerInput.canInput = false;
        }
        yield return new WaitForSeconds(animationtime);
        if (playerInput != null)
        {
            playerInput.canInput = true;
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Removes all buffs and debuffs from the caster.
    /// </summary>
    void RemoveBuffs()
    {
        BuffDebuffManager.Instance.ClearEffects(caster);
    }
}
