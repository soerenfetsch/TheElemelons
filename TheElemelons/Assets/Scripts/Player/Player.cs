using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.BranchExplorer.Layout;
using PlasticGui.WorkspaceWindow;
using UnityEngine;

/// <summary>
/// Enum to capture the player state at any given time.
/// </summary>
public enum PlayerState
{
    Walk,
    Attack,
    Interact,
    Stagger,
    Idle
}

/// <summary>
/// Represents the core player class. Manages player state, animations, and invincibility mechanics.
/// </summary>
public class Player : MonoBehaviour, ICombatant, IBuffable
{
    public int hiddenByThings = 0;
    /// <summary>
    /// Current state of the player (e.g., Walk, Attack, Idle).
    /// </summary>
    private PlayerState currentState;

    /// <summary>
    /// Animator for managing player animations.
    /// </summary>
    private Animator animator;
    /// <summary>
    /// Animator for managing wand animations.
    /// </summary>
    private Animator wandAnimator;
    public float attackFactor { get; set; }
    public float defenseFactor { get; set; }
    public bool isInvincible { get; set; }
    public ElementType elementType { get; set; }

    /// <summary>
    /// Initializes the player by setting the default state to Idle.
    /// </summary>
    public void Initialize()
    {
        SetCurrentState(PlayerState.Idle);
        attackFactor = 1f;
        defenseFactor = 1f;
        isInvincible = false;
        elementType = ElementType.None;

        animator = GetComponent<Animator>();
        wandAnimator = transform.Find("Wand_Animation").GetComponent<Animator>();
    }

    /// <summary>
    /// Unity Start method called before the first frame update.
    /// </summary>
    void Start()
    {
        Initialize();
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    /// <summary>
    /// Unity Update method called once per frame.
    /// </summary>
    void Update()
    {
        // Placeholder for player-specific update logic.
    }

    /// <summary>
    /// Retrieves the current state of the player.
    /// </summary>
    /// <returns>The current player state.</returns>
    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Sets the current state of the player.
    /// </summary>
    /// <param name="state">The new player state.</param>
    public void SetCurrentState(PlayerState state)
    {
        currentState = state;
    }

    /// <summary>
    /// Knocks the player back and staggers them for a specified duration.
    /// </summary>
    /// <param name="knockTime">The duration of the knockback effect.</param>
    public void GetKnocked(float knockTime)
    {
        StartCoroutine(OnKnockbackMoveCo(knockTime));
    }

    /// <summary>
    /// Coroutine to handle knockback logic. Sets the player state to Stagger,
    /// activates animations, and resets the state after the knockback duration.
    /// </summary>
    /// <param name="knockTime">The duration of the knockback effect.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator OnKnockbackMoveCo(float knockTime)
    {
        SetCurrentState(PlayerState.Stagger);
        animator.SetBool("Stagger", true);
        animator.SetTrigger("Stagger_trig");
        wandAnimator.SetBool("RaiseWand", false);

        yield return new WaitForSeconds(knockTime);

        SetCurrentState(PlayerState.Idle);
        animator.SetBool("Stagger", false);
    }

    public void ModifySpeed(float factor)
    {
        var movement = GetComponent<PlayerMovement>();
        movement.speedMultiplier *= factor;
    }

    public void ModifyDamage(float factor)
    {
        attackFactor *= factor;
    }

    public void ModifyDefense(float factor)
    {
        defenseFactor *= factor;
    }

    public void ModifyManaRegen(float factor)
    {
        var mana = GetComponent<Mana>();
        mana.SetManaPerRegen(mana.manaPerRegen * factor);
    }
    public bool GetIsHidden()
    {
        return hiddenByThings > 0;
    }
    public void ModifyIsHidden(float factor)
    {
        if (factor == 1)
        {
            hiddenByThings++;
        }
        else
        {
            hiddenByThings = (int)Mathf.Clamp(hiddenByThings - 1, 0f, Mathf.Infinity);
        }
    }
    /// <summary>
    /// does nothing
    /// </summary>
    /// <param name="factor"></param>
    public void ModifyIsConfused(float factor)
    { }
    public void ModifyIsFrozen(float factor)
    {
    }
}
