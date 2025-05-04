using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpearCollider : MonoBehaviour
{
    public event Action<Rigidbody2D> OnAttackTriggered; // Event for handling attacks
    public float attackCooldown = 2f; // Adjust attack cooldown as needed

    private int FRONT = 0;
    private int BACK = 1;
    private int LEFT = 2;
    private int RIGHT = 3;
    private PolygonCollider2D[] attackColliders;
    private Animator animator;
    private bool isEnabled = false; // Whether combat is enabled
    private float lastAttackTime = 0f;
    private int lastDirection;
    private bool lastEnabled;

    private void Awake()
    {
        InitializeComponents();
    }

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
    }

    /// <summary>
    /// Disables the attack collider.
    /// </summary>
    public void DisableAttack()
    {
        isEnabled = false;
    }

    private void Update()
    {
        if (animator == null) return;

        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        SelectAppropriateCollider(horizontal, vertical);
    }

    /// <summary>
    /// Chooses the attack collider based on movement direction.
    /// The four colliders correspond to FRONT, BACK, LEFT, RIGHT
    /// </summary>
    /// 
    private void SelectAppropriateCollider(float horizontal, float vertical)
    {
        int direction = FRONT;

        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical)) // Moving horizontally
        {
            direction = horizontal > 0 ? RIGHT : LEFT; // Right or Left
        }
        else // Moving vertically
        {
            direction = vertical > 0 ? BACK : FRONT; // Up or Down
        }

        // Only make changes if direction or enable changed
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
