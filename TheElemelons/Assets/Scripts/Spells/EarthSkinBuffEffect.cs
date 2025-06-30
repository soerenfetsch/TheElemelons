using System.Collections;
using UnityEngine;

/// <summary>
/// Visual and animation effect for the Earth Skin defense buff.
/// Handles animator switching, input locking, and effect expiration for the player.
/// </summary>
public class EarthSkinBuffEffect : MonoBehaviour
{
    /// <summary>
    /// Animator controller for the earthy (buffed) player.
    /// </summary>
    public RuntimeAnimatorController earthPlayerAnimator;

    /// <summary>
    /// Animator controller for the normal player.
    /// </summary>
    public RuntimeAnimatorController playerRuntimeAnimator;

    // Reference to the player GameObject.
    GameObject player;
    // Reference to the player's transform.
    Transform playerTransform;
    // Reference to the player's InputManager.
    InputManager inputManager;
    // Reference to the player's PlayerMovement.
    PlayerMovement playerMovement;
    // Reference to the player's Animator.
    Animator playerAnimator;
    // Reference to the player's Rigidbody2D.
    Rigidbody2D playerRigidBody;

    /// <summary>
    /// Initializes spelltype (Earth Notype), starts coroutine to appear, and applies defense buff visuals.
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        transform.position = playerTransform.position;
        transform.parent = playerTransform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();
        playerRigidBody = player.transform.GetComponent<Rigidbody2D>();
        inputManager = player.GetComponent<InputManager>();
        StartCoroutine(Appear());
    }

    /// <summary>
    /// Locks player input and movement for the earthskin appearance animation,
    /// then restores input after animation completes.
    /// </summary>
    IEnumerator Appear()
    {
        // Lock movement for animation
        playerMovement.SetMovement(new Vector2(0, 0));
        playerAnimator.SetFloat("Vertical", -1);
        playerAnimator.SetFloat("Horizontal", 0);
        playerRigidBody.velocity = new Vector2(0, 0);
        inputManager?.UpdateInputPossible(false);
        yield return new WaitForSeconds(1.5f);
        inputManager?.UpdateInputPossible(true);
    }

    /// <summary>
    /// Restores the player's original animator, plays the shatter animation, and destroys this effect.
    /// </summary>
    public IEnumerator Expire()
    {
        playerAnimator.runtimeAnimatorController = playerRuntimeAnimator;
        gameObject.GetComponent<Animator>().SetTrigger("Shatter");
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }

    /// <summary>
    /// Immediately restores the player's original animator and destroys this effect.
    /// </summary>
    public void InterruptSpell()
    {
        playerAnimator.runtimeAnimatorController = playerRuntimeAnimator;
        Destroy(gameObject);
    }

    /// <summary>
    /// Switches the player's animator to the earthy (buffed) animator. Triggered by animation event.
    /// </summary>
    public void Switch_to_Earthy_Animator()
    {
        playerAnimator.runtimeAnimatorController = earthPlayerAnimator;
    }
}
