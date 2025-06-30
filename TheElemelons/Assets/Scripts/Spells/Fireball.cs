using System.Collections;
using UnityEngine;

/// <summary>
/// Fireball has 3 states:
/// <list type="bullet">
/// <item><description>creating: no movement</description></item>
/// <item><description>propagating: propagates in casting direction</description></item>
/// <item><description>exploding: fireball has collided and now explodes and deals damage to enemies in collider</description></item>
/// </list>
/// </summary>
public enum FireballState
{
    /// <summary>
    /// Fireball is being created (no movement).
    /// </summary>
    creating,
    /// <summary>
    /// Fireball is propagating in the casting direction.
    /// </summary>
    propagating,
    /// <summary>
    /// Fireball is exploding and deals damage to enemies in collider.
    /// </summary>
    exploding
}

/// <summary>
/// Fireball class inherits from SpellEffect.
/// Fireball explodes after given time or collision and deals damage to enemies in collider.
/// </summary>
public class Fireball : SpellEffect
{
    [Header("Hit")]
    /// <summary>
    /// Force applied to targets when hit by the explosion.
    /// </summary>
    public float thrust = 7f;

    /// <summary>
    /// Duration of the knockback effect on targets.
    /// </summary>
    public float knockTime = 0.4f;

    /// <summary>
    /// Damage dealt by the fireball to targets.
    /// </summary>
    public float damage = 2f;

    [Header("Movement")]
    /// <summary>
    /// Time fireball spends in creating state.
    /// </summary>
    public float focusTime = 0.6f;

    /// <summary>
    /// Time fireball spends in propagating state.
    /// </summary>
    public float flyTime = 3f;

    /// <summary>
    /// Time fireball spends in exploding state.
    /// </summary>
    public float explodeTime = 0.5f;

    /// <summary>
    /// Propagation speed.
    /// </summary>
    public float moveSpeed = 1f;

    [Header("Linked Components")]
    /// <summary>
    /// Rigidbody2D component for movement.
    /// </summary>
    public Rigidbody2D myRigidbody;

    /// <summary>
    /// Animator for fireball animations.
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Transform of the casting point.
    /// </summary>
    public Transform castPoint;

    /// <summary>
    /// Collider for the fireball's hitbox.
    /// </summary>
    public BoxCollider2D boxCollider2D;

    [Header("Internal Infos")]
    /// <summary>
    /// Current fireball state.
    /// </summary>
    public FireballState currentState;

    /// <summary>
    /// Spellstype (fire fire), currentstate and orientation of fireball are set,
    /// expiration timer of fireball is set (explodes after flytime at latest),
    /// focus timer is started to get to propagating state.
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.fire, ChargeTypes.fire);
        currentState = FireballState.creating;
        castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        transform.rotation = castPoint.rotation;
        GetComponent<SpriteRenderer>().flipX = true;
        GetComponent<SpriteRenderer>().flipY = true;

        StartCoroutine(Expire());
        StartCoroutine(StartFocus());
    }

    /// <summary>
    /// Explosion triggered after flytime at latest.
    /// </summary>
    private IEnumerator Expire()
    {
        yield return new WaitForSeconds(flyTime);
        if (currentState != FireballState.exploding)
        {
            StartCoroutine(Explode());
        }
    }

    /// <summary>
    /// Switch to propagating state after focustime.
    /// </summary>
    public IEnumerator StartFocus()
    {
        yield return new WaitForSeconds(focusTime);
        currentState = FireballState.propagating;
        animator.SetTrigger("fly");
    }

    /// <summary>
    /// Explosion: update state to exploding, set rigidbody to kinematic, 
    /// velocity 0, adapt collider scale to explosion sprite and destroy GameObject after explosion time.
    /// </summary>
    private IEnumerator Explode()
    {
        currentState = FireballState.exploding;
        myRigidbody.isKinematic = true;
        myRigidbody.velocity = new Vector3(0, 0, 0);
        animator.SetTrigger("explode");
        boxCollider2D.size = new Vector2(1.95f, 2.02f);
        yield return new WaitForSeconds(explodeTime);
        Destroy(gameObject);
    }

    /// <summary>
    /// Add force to fireball in casting direction while in propagating state.
    /// </summary>
    void FixedUpdate()
    {
        if (currentState == FireballState.propagating)
        {
            myRigidbody.AddForce(transform.up * moveSpeed, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// If fireball currently propagating, collision with enemy or anything else will trigger explosion.
    /// If fireball is exploding, collision with enemy deals damage to enemy.
    /// </summary>
    /// <param name="other">The collider of the object that the spell collides with.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Spell")
            && ((other.gameObject.CompareTag("Enemy") && other.isTrigger) || !other.gameObject.CompareTag("Enemy"))
            && currentState == FireballState.propagating)
        {
            StartCoroutine(Explode());
        }
        if (currentState == FireballState.exploding && other.gameObject.CompareTag("Enemy") && other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                CombatManager.ExecuteAttack(caster, hit, spellType, difference, damage, thrust, knockTime);
            }
        }
    }

    /// <summary>
    /// Turns off the fireball's light effect after a short delay.
    /// </summary>
    public void TurnLightOff()
    {
        StartCoroutine(TurnLightOffCoroutine());
    }

    // Coroutine to turn off the light effect.
    IEnumerator TurnLightOffCoroutine()
    {
        transform.Find("Light/HalfShadowMask").gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        transform.Find("Light").gameObject.SetActive(false);
    }
}
