using UnityEngine;
using System;

/// <summary>
/// Handles attack colliders for enemies. 
/// Rotates the collider based on movement direction and triggers attack logic when the player is within range and attack is enabled.
/// </summary>
public class AttackCollider : MonoBehaviour
{
    /// <summary>
    /// Event for handling attacks, triggered when the player is hit.
    /// </summary>
    public event Action<Rigidbody2D> OnAttackTriggered; 

    /// <summary>
    /// Cooldown between attacks in seconds.
    /// </summary>
    public float attackCooldown = 2f; 

    /// <summary>
    /// Whether to adjust collider rotation based on movement direction.
    /// </summary>
    public bool adjustsColliderRotation = true; 

    // The PolygonCollider2D used for attack detection.
    private PolygonCollider2D attackCollider; 
    // Reference to the parent Animator for direction.
    private Animator animator; 
    // Whether combat is enabled.
    private bool isEnabled = true; 
    // Time of the last attack.
    private float lastAttackTime = 0f;

    /// <summary>
    /// Initializes component references.
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Finds and caches the PolygonCollider2D and the parent Animator.
    /// </summary>
    public void InitializeComponents()
    {
        attackCollider = GetComponent<PolygonCollider2D>(); 
        animator = GetComponentInParent<Animator>(); 

        if (attackCollider == null)
        {
            Debug.LogError("AttackCollider: No PolygonCollider2D found on the GameObject.");
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
        // Only attack player when last attack was more than attackCooldown seconds ago
        if (Time.time - lastAttackTime < attackCooldown) return;

        if (isEnabled && other.CompareTag("Player")) // Check if currently attacking
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                OnAttackTriggered?.Invoke(playerRb); // Trigger the event for attack logic
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
        attackCollider.enabled = true;
    }

    /// <summary>
    /// Disables the attack collider.
    /// </summary>
    public void DisableAttack()
    {
        isEnabled = false;
        attackCollider.enabled = true;
    }

    /// <summary>
    /// Updates the collider's rotation based on the animator's movement direction.
    /// </summary>
    private void Update()
    {
        if (animator == null) return;

        if (!adjustsColliderRotation) return; // Skip if rotation adjustment is disabled
        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        AdjustColliderRotation(horizontal, vertical);
    }

    /// <summary>
    /// Draws a gizmo in the editor for debugging.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    /// <summary>
    /// Rotates the attack collider based on movement direction.
    /// Default direction is bottom. Adjusts as follows:
    /// - Right: 90° clockwise
    /// - Up: 180°
    /// - Left: 270°
    /// </summary>
    /// <param name="horizontal">Horizontal movement value from animator.</param>
    /// <param name="vertical">Vertical movement value from animator.</param>
    private void AdjustColliderRotation(float horizontal, float vertical)
    {
        float angle = 0f;

        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical)) // Moving horizontally
        {
            angle = horizontal > 0 ? 90f : 270f; // Right or Left
        }
        else // Moving vertically
        {
            angle = vertical > 0 ? 180f : 0f; // Up or Down
        }

        attackCollider.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}

