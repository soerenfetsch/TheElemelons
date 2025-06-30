using System.Collections;
using UnityEngine;

/// <summary>
/// Spell that first charges up for a short time, then propagates in the casting direction.
/// Deals damage to the player if it hits the player.
/// </summary>
public class TikiGreenAttack : SpellEffect
{
    [Header("hit")]
    /// <summary>
    /// The base damage dealt by the spell.
    /// </summary>
    public float baseDamage = 0.5f;

    /// <summary>
    /// The force applied to the player when hit.
    /// </summary>
    public float thrust = 1f;

    /// <summary>
    /// The duration of knockback applied to the player.
    /// </summary>
    public float knockTime = 0.2f;

    [Header("pre-propagation movement")]
    // Transform of the caster (enemy) that created this spell.
    Transform casterTransform;
    // Offset from the caster's position to the spell's spawn point.
    Vector2 offset;

    [Header("movement")]
    // Indicates whether the spell has finished charging up and is ready to move.
    bool chargedUp = false;

    /// <summary>
    /// Time in seconds the spell charges up before propagating.
    /// </summary>
    public float chargeUpTime = 0.3f;

    /// <summary>
    /// Total lifetime of the spell before it expires.
    /// </summary>
    public float spellLifeTime = 2f;

    /// <summary>
    /// Movement speed of the spell after charging up.
    /// </summary>
    public float moveSpeed = 4f;

    // Animator for controlling spell animations.
    Animator animator;
    // SpriteRenderer for fading out the spell.
    SpriteRenderer spriteRenderer;
    // Duration of the fade-out effect before destruction.
    float fadeDuration = 0.4f;
    // Direction in which the spell will move after charging up.
    Vector2 direction;

    [Header("other")]
    // Rigidbody2D for physics interactions.
    Rigidbody2D rb;
    // Collider for detecting hits.
    BoxCollider2D boxCollider2D;

    /// <summary>
    /// Initializes spell components, calculates direction, and begins the charge-up coroutine.
    /// </summary>
    void Start()
    {
        //components
        animator = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
        casterTransform = caster.transform;
        //parameters
        Vector2 castpoint = caster.transform.position;
        if (caster.GetComponent<Tiki>() != null)
        {
            castpoint = caster.transform.GetChild(0).transform.position;
            castpoint.y += 1f;
        }
        offset = (Vector2)transform.position - castpoint;
        direction = (Vector2)transform.position - castpoint;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        spellType = new ChargeTuple(ChargeTypes.earth, ChargeTypes.notype);
        boxCollider2D.enabled = false;
        //begin charging
        StartCoroutine(ChargeUp());
    }

    /// <summary>
    /// If not charged up yet, follows the caster's position.
    /// If charged up, propagates in the casting direction.
    /// </summary>
    void FixedUpdate()
    {
        if (chargedUp)
        {
            transform.position = transform.position + (Vector3)direction * moveSpeed * Time.fixedDeltaTime;
        }
        else if (caster != null)
        {
            Vector2 newpos;
            newpos = (Vector2)caster.transform.position + offset;
            if (caster.GetComponent<Tiki>() != null)
            {
                newpos = (Vector2)caster.transform.GetChild(0).transform.position + offset;
                newpos.y += 1f;
            }
            transform.position = newpos;
        }
    }

    /// <summary>
    /// Sets chargedUp to true after the charge-up time and updates the animator.
    /// </summary>
    /// <returns>Coroutine enumerator.</returns>
    IEnumerator ChargeUp()
    {
        yield return new WaitForSeconds(chargeUpTime);
        chargedUp = true;
        animator.SetTrigger("Shoot");
        boxCollider2D.enabled = true;
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Waits until spellLifeTime expires, then fades the sprite and destroys the object.
    /// </summary>
    /// <returns>Coroutine enumerator.</returns>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        float fadeTimer = fadeDuration;
        Color c = spriteRenderer.color;
        float chunkiness = 30f;
        while (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// If collides with player and already charged up and player isn't already staggered, executes attack and self-destructs.
    /// Also destroys itself if it hits a wall.
    /// </summary>
    /// <param name="other">The collider that was entered.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (chargedUp &&
            other.gameObject.CompareTag("Player") &&
            other.isTrigger &&
            other.gameObject.GetComponent<Player>().GetCurrentState() != PlayerState.Stagger)
        {
            Vector2 attackDirection = other.transform.position - transform.position;
            CombatManager.ExecuteAttack(caster, other.GetComponent<Rigidbody2D>(), spellType, attackDirection, baseDamage, thrust, knockTime);
            Destroy(gameObject);
        }
        else if (chargedUp &&
            other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
