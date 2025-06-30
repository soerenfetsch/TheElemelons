using System.Collections;
using UnityEngine;

/// <summary>
/// Enum to capture the player state at any given time.
/// </summary>
public enum PlayerState
{
    /// <summary>Player is walking.</summary>
    Walk,
    /// <summary>Player is attacking.</summary>
    Attack,
    /// <summary>Player is interacting with an object or NPC.</summary>
    Interact,
    /// <summary>Player is staggered (e.g., after knockback).</summary>
    Stagger,
    /// <summary>Player is idle.</summary>
    Idle
}

/// <summary>
/// Represents the core player class. Manages player state, animations, and invincibility mechanics.
/// Implements <see cref="ICombatant"/> and <see cref="IBuffable"/> for combat and buff/debuff interactions.
/// </summary>
public class Player : MonoBehaviour, ICombatant, IBuffable
{
    /// <summary>
    /// Number of sources currently hiding the player (e.g., stealth effects).
    /// </summary>
    public int hiddenByThings = 0;

    /// <summary>
    /// Number of sources currently freezing the player (e.g., freeze debuffs).
    /// </summary>
    public int frozenByThings = 0;

    // Reference to the input manager for enabling/disabling input.
    InputManager inputManager;

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

    /// <summary>
    /// Attack factor multiplier for the player (affects outgoing damage).
    /// </summary>
    public float attackFactor { get; set; }

    /// <summary>
    /// Defense factor multiplier for the player (affects incoming damage).
    /// </summary>
    public float defenseFactor { get; set; }

    /// <summary>
    /// Whether the player is currently invincible.
    /// </summary>
    public bool isInvincible { get; set; }

    /// <summary>
    /// The elemental type of the player (used for elemental interactions).
    /// </summary>
    public ElementType elementType { get; set; }

    /// <summary>
    /// Initializes the player by setting the default state to Idle and default stats.
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
        inputManager = GetComponent<InputManager>();
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
    /// Handles input freezing if the player is frozen.
    /// </summary>
    void Update()
    {
        // Placeholder for player-specific update logic.
        inputManager?.UpdateInputPossible(frozenByThings <= 0);
    }

    /// <summary>
    /// Retrieves the current state of the player.
    /// </summary>
    /// <returns>The current <see cref="PlayerState"/>.</returns>
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

    /// <summary>
    /// Modifies the player's movement speed by a given factor.
    /// </summary>
    /// <param name="factor">The factor to multiply the speed by.</param>
    public void ModifySpeed(float factor)
    {
        var movement = GetComponent<PlayerMovement>();
        movement.speedMultiplier *= factor;
    }

    /// <summary>
    /// Modifies the player's attack factor by a given factor.
    /// </summary>
    /// <param name="factor">The factor to multiply the attack by.</param>
    public void ModifyDamage(float factor)
    {
        attackFactor *= factor;
    }

    /// <summary>
    /// Modifies the player's defense factor by a given factor.
    /// </summary>
    /// <param name="factor">The factor to multiply the defense by.</param>
    public void ModifyDefense(float factor)
    {
        defenseFactor *= factor;
    }

    /// <summary>
    /// Modifies the player's mana regeneration rate by a given factor.
    /// </summary>
    /// <param name="factor">The factor to multiply the mana regen by.</param>
    public void ModifyManaRegen(float factor)
    {
        var mana = GetComponent<Mana>();
        mana.SetManaPerRegen(mana.manaPerRegen * factor);
    }

    /// <summary>
    /// Returns true if the player is currently hidden (e.g., by stealth effects).
    /// </summary>
    /// <returns>True if hidden, false otherwise.</returns>
    public bool GetIsHidden()
    {
        return hiddenByThings > 0;
    }

    /// <summary>
    /// Modifies the player's hidden state (increments or decrements the hidden counter).
    /// </summary>
    /// <param name="factor">1 to increment, 0 to decrement.</param>
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
    /// Does nothing. Player cannot be confused (stub for IBuffable).
    /// </summary>
    /// <param name="factor">Unused.</param>
    public void ModifyIsConfused(float factor)
    { }

    /// <summary>
    /// Modifies the player's frozen state (increments or decrements the frozen counter).
    /// </summary>
    /// <param name="factor">1 to increment, 0 to decrement.</param>
    public void ModifyIsFrozen(float factor)
    {
        if (factor == 1)
        {
            frozenByThings++;
        }
        else
        {
            frozenByThings = (int)Mathf.Clamp(frozenByThings - 1, 0f, Mathf.Infinity);
        }
    }
}
