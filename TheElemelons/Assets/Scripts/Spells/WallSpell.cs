using System.Collections;
using UnityEngine;

/// <summary>
/// WallSpell inherits from SpellEffect.
/// Spawns a wall for a given time, oriented based on the player's position.
/// </summary>
public class WallSpell : SpellEffect
{
    /// <summary>
    /// Duration the wall remains before sinking.
    /// </summary>
    public float spellLifeTime = 7f;

    /// <summary>
    /// Distance from the player to spawn the wall.
    /// </summary>
    public float distance = 3f;

    // Reference to the player's transform.
    Transform playerTransform;
    // Reference to the wall's BoxCollider2D.
    BoxCollider2D boxCollider2D;
    // Reference to the wall's Animator.
    Animator animator;

    /// <summary>
    /// Sets spell type (earth, earth), orients the wall, and starts the expiration coroutine.
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.earth, ChargeTypes.earth);
        playerTransform = GameObject.Find("Player").transform;
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        Orientate();
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Orients the wall in the correct direction according to the player's position,
    /// adapts the collider and triggers the correct animation.
    /// </summary>
    void Orientate()
    {
        Vector2 direction = transform.position - playerTransform.position;
        direction.Normalize();

        if (direction.x < -0.707 && direction.y > -0.707 && direction.y < 0.707)
        {
            // Left
            animator.SetInteger("Direction", -1);
            boxCollider2D.offset = new Vector2(0.01683933f, 0.9102298f);
            boxCollider2D.size = new Vector2(0.5423633f, 1.82007f);
        }
        else if (direction.x > 0.707 && direction.y > -0.707 && direction.y < 0.707)
        {
            // Right
            animator.SetInteger("Direction", 1);
            boxCollider2D.offset = new Vector2(-0.09968555f, 0.9684917f);
            boxCollider2D.size = new Vector2(0.513232f, 1.878331f);
        }
        boxCollider2D.enabled = true;
    }

    /// <summary>
    /// Triggers the sink animation after spellLifeTime and destroys the wall GameObject.
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        animator.SetTrigger("Sink");
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
