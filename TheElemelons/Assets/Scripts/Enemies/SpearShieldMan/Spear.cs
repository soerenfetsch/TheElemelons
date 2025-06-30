using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages a spear projectile, handling its movement, collision, and lifetime.
/// </summary>
/// <remarks>
/// This script expects to be on a GameObject that also has a Rigidbody2D and a BoxCollider2D.
/// The spear is inactive until the ThrowSpear() method is called.
/// </remarks>
public class Spear : MonoBehaviour
{
    /// <summary>
    /// Event fired when the spear collides with an object tagged as "Player".
    /// </summary>
    /// <param name="playerRb">The Rigidbody2D of the player that was hit.</param>
    /// <param name="attackDirection">The direction vector the spear was traveling in when it hit.</param>
    public event Action<Rigidbody2D, Vector2> OnAttackTriggered;

    // --- Private State ---

    /// <summary>
    /// Internal flag to ensure the spear only triggers a hit after being explicitly thrown.
    /// This prevents OnTriggerStay2D from firing before the throw is initiated.
    /// </summary>
    private bool isEnabled = false;

    /// <summary>
    /// Stores the direction of the spear's travel once thrown.
    /// </summary>
    private Vector2 spearDirection;

    /// <summary>
    /// Cached reference to the spear's BoxCollider2D component.
    /// </summary>
    private BoxCollider2D spearCollider;

    /// <summary>
    /// Initializes the component by caching the collider and ensuring it starts in a disabled state.
    /// </summary>
    void Start()
    {
        spearCollider = GetComponent<BoxCollider2D>();
        spearCollider.enabled = false; // Disable the spear collider at the start
    }

    /// <summary>
    /// Launches the spear, configuring its physics, setting its velocity, and starting its self-destruct timer.
    /// </summary>
    /// <param name="direction">The vector direction to throw the spear.</param>
    /// <param name="speed">The speed (units per second) at which the spear will travel.</param>
    /// <param name="time">The duration in seconds before the spear destroys itself if it hits nothing.</param>
    public void ThrowSpear(Vector2 direction, float speed, float time)
    {
        isEnabled = true; // Enable the spear's logic now that it has been thrown.
        spearDirection = direction;

        // NOTE: GetComponent is called here every time the spear is thrown.
        // For better performance, this would ideally be cached in Start() or Awake().
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Spear: No Rigidbody2D found on the GameObject.");
            return;
        }

        // --- Configure Rigidbody for projectile motion ---
        rb.isKinematic = false; // Set the Rigidbody to be dynamic so it can move via velocity.
        rb.gravityScale = 0;    // Disable gravity for predictable flight.
        rb.angularDrag = 0;     // Disable angular drag.
        rb.drag = 0;            // Disable linear drag.
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent spinning during flight.

        // Set the spear's velocity to start it moving.
        rb.velocity = direction.normalized * speed;

        // --- Orient the spear to face its direction of travel ---
        // The -90 degree offset is a common adjustment for sprites that are drawn facing "up" by default.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Enable the collider to allow hit detection.
        spearCollider.enabled = true;

        // Start the self-destruct timer.
        StartCoroutine(DestroySpearAfterTime(time));
    }

    /// <summary>
    /// Coroutine that destroys the spear GameObject after a specified delay.
    // </summary>
    /// <param name="time">The lifetime of the spear in seconds.</param>
    private IEnumerator DestroySpearAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        spearCollider.enabled = false;
        Destroy(gameObject); // Destroy the spear after the specified time
    }

    /// <summary>
    /// Called by Unity every physics frame if the spear's collider is overlapping another.
    /// </summary>
    /// <param name="other">The other Collider2D involved in the collision.</param>
    private void OnTriggerStay2D(Collider2D other)
    {
        // Only check for collisions if the spear has been actively thrown.
        if (isEnabled && other.CompareTag("Player"))
        {
            // NOTE: GetComponent is called on the other object during the collision check.
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Trigger the event for external systems to handle the attack logic.
                OnAttackTriggered?.Invoke(playerRb, spearDirection);
                Destroy(gameObject); // Destroy the spear after hitting the player
            }
        }
    }
}