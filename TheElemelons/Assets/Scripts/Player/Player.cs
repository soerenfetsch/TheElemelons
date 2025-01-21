using System.Collections;
using System.Collections.Generic;
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
public class Player : MonoBehaviour
{
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
    private Animator wand_animator;

    /// <summary>
    /// Indicates whether the player is invincible.
    /// </summary>
    private bool isInvincible = false;

    /// <summary>
    /// Factor to scale the damage dealt by the player.
    /// </summary>
    private float damageFactor = 1f;

    /// <summary>
    /// Factor to scale the damage received by the player.
    /// </summary>
    private float defenseFactor = 1f;

    /// <summary>
    /// Initializes the player by setting the default state to Idle.
    /// </summary>
    public void Initialize()
    {
        SetCurrentState(PlayerState.Idle);
    }

    /// <summary>
    /// Unity Start method called before the first frame update.
    /// </summary>
    void Start()
    {
        Initialize();
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
        wand_animator.SetBool("RaiseWand", false);

        yield return new WaitForSeconds(knockTime);

        SetCurrentState(PlayerState.Idle);
        animator.SetBool("Stagger", false);
    }

    /// <summary>
    /// Checks whether the player is currently invincible.
    /// </summary>
    /// <returns>True if the player is invincible, false otherwise.</returns>
    public bool IsInvincible()
    {
        return isInvincible;
    }

    /// <summary>
    /// Sets the player's invincibility status.
    /// </summary>
    /// <param name="invincible">True to make the player invincible, false otherwise.</param>
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    /// <summary>
    /// Retrieves the player's damage scaling factor.
    /// </summary>
    /// <returns>The damage factor.</returns>
    public float GetDamageFactor()
    {
        return damageFactor;
    }

    /// <summary>
    /// Sets the player's damage scaling factor.
    /// </summary>
    /// <param name="factor">The new damage factor.</param>
    public void SetDamageFactor(float factor)
    {
        damageFactor = factor;
    }

    /// <summary>
    /// Retrieves the player's defense scaling factor.
    /// </summary>
    /// <returns>The defense factor.</returns>
    public float GetDefenseFactor()
    {
        return defenseFactor;
    }

    /// <summary>
    /// Sets the player's defense scaling factor.
    /// </summary>
    /// <param name="factor">The new defense factor.</param>
    public void SetDefenseFactor(float factor)
    {
        defenseFactor = factor;
    }
}
