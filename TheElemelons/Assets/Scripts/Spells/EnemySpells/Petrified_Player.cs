using System.Collections;
using UnityEngine;

/// <summary>
/// Visual and animation effect for the petrified (stone) debuff on the player.
/// Handles animator switching, movement locking, and effect expiration for the player.
/// </summary>
public class Petrified_Player : MonoBehaviour
{
    // Reference to the player's transform.
    Transform playerTransform;
    // Reference to the player's InputManager.
    InputManager inputManager;
    // Animator for the petrified effect.
    Animator animator;
    // Small vertical offset to position the effect below the player.
    float offset = 0.01f;

    /// <summary>
    /// Initializes references, sets up petrified animation state, and locks player movement.
    /// </summary>
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

        animator = GetComponent<Animator>();
        Animator playerAnimator = playerTransform.GetComponent<Animator>();
        AnimatorClipInfo[] playeranimatorinfo = playerAnimator.GetCurrentAnimatorClipInfo(0);
        string current_wand_animation = playeranimatorinfo[0].clip.name;
        if (current_wand_animation != "idle")
        {
            animator.SetBool("Wand", true);
        }
        Vector2 dir = new Vector2(playerAnimator.GetFloat("Horizontal"), playerAnimator.GetFloat("Vertical"));
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
        transform.position = playerTransform.position - new Vector3(0, offset, 0);
        playerTransform.GetComponent<PlayerMovement>().SetMovement(Vector2.zero);
        transform.SetParent(playerTransform, worldPositionStays: true);
    }

    /// <summary>
    /// Plays the decompose animation and destroys the petrified effect after a delay.
    /// </summary>
    /// <returns>Coroutine enumerator.</returns>
    public IEnumerator Expire()
    {
        animator.SetTrigger("Decompose");
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
