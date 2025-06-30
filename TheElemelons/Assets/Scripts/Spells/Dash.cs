using System.Collections;
using UnityEngine;

/// <summary>
/// Dash inherits from SpellEffect.
/// Player is propelled in the direction they were facing for a given time.
/// Player cannot input movement while dash is active.
/// </summary>
public class Dash : SpellEffect
{
    /// <summary>
    /// Duration of the dash in seconds.
    /// </summary>
    public float dashTime = 0.3f;

    /// <summary>
    /// Distance to dash in world units.
    /// </summary>
    public float dashDistance = 5f;

    // Calculated dash speed based on distance and time.
    float dashSpeed;

    // Reference to the player's movement script.
    PlayerMovement playerMovement;
    // Reference to the player's Rigidbody2D.
    Rigidbody2D playerRigidBody;
    // Reference to the player's input manager.
    InputManager inputManager;
    // Reference to the player's transform.
    Transform playerTransform;
    // Reference to the player GameObject.
    GameObject player;
    // Reference to the dash animator.
    Animator animator;
    // Direction vector for the dash.
    Vector2 move;

    /// <summary>
    /// Initializes spell type (notype, wind) and starts the dash coroutine.
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerRigidBody = player.transform.GetComponent<Rigidbody2D>();
        inputManager = player.GetComponent<InputManager>();
        spellType = new ChargeTuple(ChargeTypes.notype, ChargeTypes.wind);
        dashSpeed = dashDistance / dashTime;
        animator = gameObject.GetComponent<Animator>();
        StartCoroutine(DoDash());
    }

    /// <summary>
    /// Locks movement inputs for the player, computes dash movement vector, passes values to dash animator, waits out dash time,
    /// then destroys the dash GameObject.
    /// </summary>
    IEnumerator DoDash()
    {   
        // Optionally reset movement: playerMovement.SetMovement(new Vector2(0,0));
        inputManager.canMove = false;
        move = new Vector2(player.GetComponent<Animator>().GetFloat("Horizontal"), player.GetComponent<Animator>().GetFloat("Vertical"));
        animator.SetFloat("Horizontal", move.x);
        animator.SetFloat("Vertical", move.y);
        move.Normalize();
        Physics2D.IgnoreLayerCollision(3, 8);
        yield return new WaitForSeconds(dashTime);
        Physics2D.IgnoreLayerCollision(3, 8, false);
        inputManager.canMove = true;
        Destroy(gameObject);
    }

    /// <summary>
    /// Moves the player in the given direction (calculated in DoDash) and at the given speed.
    /// Updates dash GameObject position for the animation.
    /// </summary>
    void FixedUpdate()
    {
        playerRigidBody.MovePosition(playerRigidBody.position + move * dashSpeed / playerMovement.GetMoveSpeed() * Time.fixedDeltaTime);
        transform.position = new Vector2(playerTransform.position.x, playerTransform.position.y - 2);
    }
}
