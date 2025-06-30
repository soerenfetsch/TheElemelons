using System;
using UnityEngine;

/// <summary>
/// Handles attack colliders for the SpearShieldMan enemy.
/// Switches between four PolygonCollider2Ds (front, back, left, right) based on movement direction,
/// and triggers attack logic when the player is within range and attack is enabled.
/// </summary>
public class ShieldSpearCollider : MonoBehaviour
{
    /// <summary>
    /// Event triggered when an attack lands on the player.
    /// </summary>
    public event Action<Rigidbody2D> OnAttackTriggered;

    /// <summary>
    /// Cooldown between attacks in seconds.
    /// </summary>
    public float attackCooldown = 2f;

    // Indices for the four attack directions.
    private int FRONT = 0;
    private int BACK = 1;
    private int LEFT = 2;
    private int RIGHT = 3;

    // Array of PolygonCollider2Ds for each direction.
    private PolygonCollider2D[] attackColliders;
    // Reference to the parent Animator for direction.
    private Animator animator;
    // Whether the attack collider is currently enabled.
    private bool isEnabled = false;
    // Time of the last attack.
    private float lastAttackTime = 0f;
    // Last direction index used.
    private int lastDirection;
    // Last enabled state.
    private bool lastEnabled;

    /// <summary>
    /// Initializes component references.
    /// </summary>
    private void Start()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Finds and caches PolygonCollider2Ds and the parent Animator.
    /// </summary>
    public void InitializeComponents()
    {
        attackColliders = GetComponents<PolygonCollider2D>();
        animator = GetComponentInParent<Animator>();

        if (attackColliders == null)
        {
            Debug.LogError("AttackCollider: No PolygonCollider2Ds found on the GameObject.");
        }
        if (animator == null)
        {
            Debug.LogError("AttackCollider: No Animator found on the parent.");
        }
    }

    /// <summary>
    /// Handles attack logic when the player stays within the collider.
    /// Triggers attack if cooldown has elapsed and attack is enabled.
    /// </summary>
    /// <param name="other">The collider that stays within the trigger.</param>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        if (isEnabled && other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                OnAttackTriggered?.Invoke(playerRb);
                lastAttackTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Enables the attack collider.
    /// </summary>
    public void EnableAttack()
    {
        isEnabled = true;
    }

    /// <summary>
    /// Disables the attack collider.
    /// </summary>
    public void DisableAttack()
    {
        isEnabled = false;
    }

    /// <summary>
    /// Checks if the attack collider is enabled.
    /// </summary>
    /// <returns>True if enabled, false otherwise.</returns>
    public bool IsEnabled()
    {
        return isEnabled;
    }

    /// <summary>
    /// Updates the active collider based on the animator's movement direction.
    /// </summary>
    private void Update()
    {
        if (animator == null) return;

        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        SelectAppropriateCollider(horizontal, vertical);
    }

    /// <summary>
    /// Chooses the attack collider based on movement direction.
    /// The four colliders correspond to FRONT, BACK, LEFT, RIGHT.
    /// </summary>
    /// <param name="horizontal">Horizontal movement value from animator.</param>
    /// <param name="vertical">Vertical movement value from animator.</param>
    private void SelectAppropriateCollider(float horizontal, float vertical)
    {
        int direction = FRONT;

        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            direction = horizontal > 0 ? RIGHT : LEFT;
        }
        else
        {
            direction = vertical > 0 ? BACK : FRONT;
        }

        // Only update if direction or enabled state changed
        if (isEnabled == lastEnabled && direction == lastDirection)
            return;

        // Enable only the appropriate collider if isEnabled is set
        attackColliders[FRONT].enabled = false;
        attackColliders[BACK].enabled = false;
        attackColliders[LEFT].enabled = false;
        attackColliders[RIGHT].enabled = false;
        attackColliders[direction].enabled = isEnabled;

        lastEnabled = isEnabled;
        lastDirection = direction;
    }
}
