using UnityEngine;

/// <summary>
/// Manages player movement, including physics, animations, and speed scaling.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    /// <summary>
    /// The base movement speed of the player.
    /// </summary>
    public float baseSpeed = 5f;

    /// <summary>
    /// Multiplier for adjusting the player's speed dynamically.
    /// </summary>
    public float speedMultiplier = 1f;

    [Header("Hurtbox")]
    /// <summary>
    /// The hurtbox collider for the player, used for detecting damage.
    /// </summary>
    public BoxCollider2D hurtbox;

    [Header("Internal Components")]
    /// <summary>
    /// Reference to the Player script for managing state.
    /// </summary>
    private Player player;

    /// <summary>
    /// Rigidbody2D component for handling physics-based movement.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Animator for managing movement-related animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Animator for managing wand-related animations.
    /// </summary>
    private Animator wandAnimator;

    /// <summary>
    /// The current movement vector of the player.
    /// </summary>
    private Vector2 movement;

    /// <summary>
    /// Unity Start method called before the first frame update.
    /// Initializes the player and movement-related components.
    /// </summary>
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes player movement variables and sets default states.
    /// </summary>
    public void Initialize()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        wandAnimator = transform.Find("Wand_Animation").GetComponent<Animator>();
        SetMovement(Vector2.zero);
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", -1);
    }

    /// <summary>
    /// Handles player movement input and updates animations.
    /// </summary>
    public void OnInputMove()
    {
        // Updates the speed parameter in the animator.
        animator.SetFloat("Speed", GetMovement().sqrMagnitude);

        // Handles movement physics and animations if the player is not staggered.
        if (GetMovement() != Vector2.zero && player.GetCurrentState() != PlayerState.Stagger)
        {
            player.SetCurrentState(PlayerState.Walk);
            movement.Normalize();

            // Moves the Rigidbody2D based on the current movement vector and speed.
            rb.MovePosition(rb.position + GetMovement() * GetMoveSpeed() * Time.fixedDeltaTime);

            // Updates movement-related animations.
            animator.SetFloat("Horizontal", GetMovement().x);
            animator.SetFloat("Vertical", GetMovement().y);
            wandAnimator.SetFloat("Horizontal", GetMovement().x);
            wandAnimator.SetFloat("Vertical", GetMovement().y);
        }
        else if (player.GetCurrentState() != PlayerState.Stagger && GetMovement() == Vector2.zero)
        {
            player.SetCurrentState(PlayerState.Idle);
        }
    }

    /// <summary>
    /// Unity FixedUpdate method called at fixed intervals for physics calculations.
    /// Invokes OnInputMove to process player movement.
    /// </summary>
    void FixedUpdate()
    {
        OnInputMove();
    }

    /// <summary>
    /// Sets the player's movement vector.
    /// </summary>
    /// <param name="newMovement">The new movement vector.</param>
    public void SetMovement(Vector2 newMovement)
    {
        movement = newMovement;
    }

    /// <summary>
    /// Retrieves the current movement vector.
    /// </summary>
    /// <returns>The current movement vector.</returns>
    public Vector2 GetMovement()
    {
        return movement;
    }

    /// <summary>
    /// Retrieves the player's current movement speed.
    /// </summary>
    /// <returns>The current movement speed, adjusted by the speed multiplier.</returns>
    public float GetMoveSpeed()
    {
        return baseSpeed * speedMultiplier;
    }
}
