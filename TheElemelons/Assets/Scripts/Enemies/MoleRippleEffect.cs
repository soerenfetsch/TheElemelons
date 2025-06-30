using System;
using UnityEngine;

/// <summary>
/// Visual effect for the Mole's ripple attack. Moves in a set direction and triggers an event when hitting the player.
/// </summary>
[RequireComponent(typeof(Animator), typeof(PolygonCollider2D))]
public class RippleEffect : MonoBehaviour
{
    [Header("Ripple Settings")]
    /// <summary>
    /// Speed at which the ripple moves.
    /// </summary>
    public float speed = 3f;

    // Direction in which the ripple moves.
    private Vector2 direction = Vector2.down;

    /// <summary>
    /// Event triggered when the ripple hits the player.
    /// </summary>
    public event Action<Rigidbody2D, RippleEffect> OnPlayerHit;

    // Rigidbody2D for movement.
    private Rigidbody2D rb;

    /// <summary>
    /// Initializes the Rigidbody2D component.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Moves the ripple in its set direction every frame.
    /// </summary>
    private void Update()
    {
        // Move ripple in direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    /// <summary>
    /// Triggers the OnPlayerHit event if the ripple collides with the player.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
        if (other.CompareTag("Player") && hit != null)
        {
            OnPlayerHit?.Invoke(hit, this);
        }
    }

    /// <summary>
    /// Destroys the ripple effect. Call this from an animation event at the end of the ripple animation.
    /// </summary>
    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
