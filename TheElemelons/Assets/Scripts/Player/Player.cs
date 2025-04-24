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
public class Player : MonoBehaviour, ICombatant, IBuffable
{
    /// <summary>
    /// Singleton instance of the Player class.
    /// </summary>
    public static Player Instance { get; private set; }

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

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        // Initialize player
        Initialize();
    }

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

    /// <summary>
    /// Restores player state from saved data.
    /// </summary>
    /// <param name="playerData">The saved player data.</param>
    public void RestoreFromSave(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogWarning("No player data to restore.");
            return;
        }

        // Restore position
        transform.position = playerData.position;
        
        // Restore health
        var healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.SetHealth(playerData.currentHealth);
        }
        
        // Restore mana
        var manaComponent = GetComponent<Mana>();
        if (manaComponent != null)
        {
            manaComponent.SetMana(playerData.currentMana);
        }
        
        // Restore attack and defense factors
        attackFactor = playerData.attackFactor;
        defenseFactor = playerData.defenseFactor;
        
        // Restore element type
        elementType = (ElementType)playerData.elementType;
        
        // Reset state to idle
        SetCurrentState(PlayerState.Idle);
        
        Debug.Log("Player state restored from save data.");
    }

    /// <summary>
    /// Resets the player to default state.
    /// </summary>
    public void ResetPlayer()
    {
        // Reset position to origin or spawn point
        transform.position = Vector3.zero;
        
        // Reset health to maximum
        var healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.SetHealth(healthComponent.GetMaxHealth());
        }
        
        // Reset mana to maximum
        var manaComponent = GetComponent<Mana>();
        if (manaComponent != null)
        {
            manaComponent.SetMana(manaComponent.GetMaxMana());
        }
        
        // Reset combat factors
        attackFactor = 1f;
        defenseFactor = 1f;
        isInvincible = false;
        
        // Reset element
        elementType = ElementType.None;
        
        // Reset state
        SetCurrentState(PlayerState.Idle);
        
        Debug.Log("Player has been reset to default state.");
    }
}
