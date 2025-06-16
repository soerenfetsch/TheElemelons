using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Abstract base class for collectable items. Handles general initialization, movement, and player interaction logic.
/// Derived classes implement specific OnCollect behavior.
=======
/// Abstract base class for all collectable items in the game.
/// Handles general logic for hovering, following the player, and triggering collection effects.
>>>>>>> Stashed changes
/// </summary>
public abstract class Collectable : MonoBehaviour
{
    // Force applied to the collectable when pulled toward the player.
    protected float force = 10f;
    // Range within which the collectable is attracted to the player.
    protected float range = 1f;
    // Reference to the player's Rigidbody2D.
    protected Rigidbody2D player;
    // Reference to the player's Transform.
    protected Transform playerTransform;
    // Reference to the player's Health component.
    protected Health playerHealth;
    // Reference to the player's Mana component.
    protected Mana playerMana;
    // Animator for collectable animations.
    protected Animator animator;
    // Rigidbody2D for physics interactions.
    protected Rigidbody2D rb;
    // Whether this collectable has already been collected.
    protected bool collected;
    // Collider for detecting collection.
    protected BoxCollider2D boxCollider2D;
    // SpriteRenderer for visual effects.
    protected SpriteRenderer spriteRenderer;

    [Header("Hovering Motion")]
    // Amplitude of the hovering motion.
    protected float amplitude = 0.1f;
    // Frequency of the hovering motion.
    protected float frequency = 0.3f;
    // The fixed point around which the collectable hovers.
    protected Vector2 fixpoint;
    /// <summary>
    /// Prefab for the collectable's shadow.
    /// </summary>
    public GameObject shadowObject;
    // Transform for the shadow instance.
    protected Transform shadow;
    // Vertical offset for the shadow.
    protected float shadowOffset = 0.7f;
    // Scale of the shadow.
    protected float shadowScale = 1f;

    /// <summary>
<<<<<<< Updated upstream
    /// Initializes references to player, rigidbody, shadow, animator, and other components.
=======
    /// Initializes references to player, rigidbody, colliders, animator, and shadow.
>>>>>>> Stashed changes
    /// </summary>
    public void InitializeGeneral()
    {
        player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        fixpoint = rb.position;
        shadow = Instantiate(shadowObject, transform.position - Vector3.down * shadowOffset, Quaternion.Euler(0, 0, 0)).transform;
        shadow.parent = transform;
        shadow.localScale = new Vector3(shadowScale, shadowScale, 1);
        boxCollider2D = GetComponent<BoxCollider2D>();
        playerTransform = player.transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Object is pulled towards player if in range (~2 units), else performs a hovering motion on a fixed location.
    /// If already collected, follows the player.
    /// </summary>
    protected void Movement()
    {
        if (!collected)
        {
            Vector3 difference = player.position - rb.position;
            if (Mathf.Abs(difference.magnitude) < range)
            {
                rb.AddForce(difference.normalized * (range - difference.magnitude) * force);
                fixpoint = rb.position;
            }
            else
            {
                Vector2 moving_by = Vector2.up * Mathf.Sin(frequency * Time.frameCount * Time.fixedDeltaTime) * amplitude;
                shadow.position = fixpoint - Vector2.up * shadowOffset;
                rb.MovePosition(fixpoint + moving_by);
            }
        }
        else
        {
            transform.position = player.position;
        }
    }

    /// <summary>
    /// Calls the abstract method <see cref="OnCollect"/> if collision with the player is detected.
    /// </summary>
<<<<<<< Updated upstream
    /// <param name="other">Collider that entered the trigger.</param>
=======
    /// <param name="other">The collider that entered the trigger.</param>
>>>>>>> Stashed changes
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger)
        {
            OnCollect();
        }
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Abstract method to define collectable-specific behavior when collected by the player.
=======
    /// Called when the collectable is collected by the player.
    /// Must be implemented by derived classes to define specific collection behavior.
>>>>>>> Stashed changes
    /// </summary>
    protected abstract void OnCollect();
}
