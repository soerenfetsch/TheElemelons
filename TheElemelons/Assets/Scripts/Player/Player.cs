using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

/// <summary>
/// Enum to capture the player state at any given time
/// </summary>
public enum PlayerState
{
    Walk,
    Attack,
    Interact,
    Stagger,
    Idle
}

public class Player : MonoBehaviour
{
    private PlayerState currentState;
    private Animator animator;
    private Animator wand_animator;
    private bool isInvincible = false;
    private float damageFactor = 1f;
    private float defenseFactor = 1f;

    public void Initialize()
    {
        SetCurrentState(PlayerState.Idle);
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Get the current state of the player
    /// </summary>
    /// <returns>PlayerState</returns>
    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Set the current state of the player
    /// </summary>
    /// <param name="state">PlayerState</param>
    public void SetCurrentState(PlayerState state)
    {
        currentState = state;
    }

    /// <summary>
    /// Knock out the player for a given amount of time
    /// </summary>
    /// <param name="knockTime"></param>
    public void GetKnocked(float knockTime)
    {
        StartCoroutine(OnKnockbackMoveCo(knockTime));
    }

    /// <summary>
    /// Coroutine to set the player state to stagger during the knockTime
    /// Activates stagger animation and after time resets state and animation.
    /// </summary>
    /// <param name="knockTime"></param>
    /// <returns></returns>
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

    public bool IsInvincible()
    {
        return isInvincible;
    }

    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    public float GetDamageFactor()
    {
        return damageFactor;
    }

    public void SetDamageFactor(float factor)
    {
        damageFactor = factor;
    }

    public float GetDefenseFactor()
    {
        return defenseFactor;
    }

    public void SetDefenseFactor(float factor)
    {
        defenseFactor = factor;
    }
}
