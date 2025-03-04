using UnityEngine;
using System;

public class AttackCollider : MonoBehaviour
{
    public event Action<Rigidbody2D> OnAttackTriggered; // Event for handling attacks
    public float attackCooldown = 2f; // Adjust attack cooldown as needed

    private PolygonCollider2D attackCollider; 
    private Animator animator; 
    private bool isEnabled = true; // Whether combat is enabled
    private float lastAttackTime = 0f;

    private void Awake()
    {
        InitializeComponents();
    }

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

    private void OnTriggerEnter2D(Collider2D other)
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

    private void Update()
    {
        if (animator == null) return;

        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        AdjustColliderRotation(horizontal, vertical);
    }

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

