using System.Collections;
using UnityEngine;
/// <summary>
/// FireTornado class inherits from SpellEffect.
/// FireTornado first moves in casting direction and can randomly change direction or bounce off walls.
/// </summary>
public class FireTornado : SpellEffect
{
    [Header("Movement")]
    /// <summary>
    /// Lifetime of the spell before it expires.
    /// </summary>
    public float spellLifeTime = 3f;

    /// <summary>
    /// Time for the disappear animation before destruction.
    /// </summary>
    public float spellDeathTime = 0.3f;

    /// <summary>
    /// Movement speed of the tornado.
    /// </summary>
    public float moveSpeed = 1f;

    /// <summary>
    /// Direction correction factor if direction is changed.
    /// </summary>
    public float correction = 0.1f;

    /// <summary>
    /// Time between random direction switches.
    /// </summary>
    public float switchTime = 1f;

    /// <summary>
    /// Constraints for the random direction switch interval.
    /// </summary>
    public Vector2 switchTimeConstraints = new Vector2(1f, 3f);

    [Header("Hit")]
    /// <summary>
    /// Force applied to targets when hit by the tornado.
    /// </summary>
    public float thrust = 7f;

    /// <summary>
    /// Duration of the knockback effect on targets.
    /// </summary>
    public float knockTime = 0.4f;

    /// <summary>
    /// Damage dealt by the tornado to targets.
    /// </summary>
    public float damage = 2f;

    [Header("Linked Components")]
    /// <summary>
    /// Rigidbody2D component for movement.
    /// </summary>
    public Rigidbody2D myRigidbody;

    // Reference to the casting point transform.
    Transform castPoint;

    [Header("Internal Stuff")]
    /// <summary>
    /// Current movement direction.
    /// </summary>
    Vector2 direction;

    // Animator for tornado animations.
    Animator animator;

    /// <summary>
    /// Sets spell type (fire wind), initial direction, starts expiration timer, and random direction switching coroutine.
    /// </summary>
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        spellType = new ChargeTuple(ChargeTypes.fire, ChargeTypes.wind);
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        direction = castPoint.transform.up;
        StartCoroutine(Expire());
        StartCoroutine(SwitchDirection());
    }

    /// <summary>
    /// Coroutine for random direction switching.
    /// </summary>
    IEnumerator SwitchDirection()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchTime);
            switchTime = Random.Range(switchTimeConstraints.x, switchTimeConstraints.y);
            direction.x = Random.Range(-1, 1);
            direction.y = Random.Range(-1, 1);
            direction.Normalize();
        }
    }

    /// <summary>
    /// Starts expiration timer and self-destructs at the end.
    /// </summary>
    private IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        StartCoroutine(SelfDestroy());
    }

    /// <summary>
    /// Adds force in the current direction.
    /// </summary>
    void FixedUpdate()
    {
        direction.x = myRigidbody.velocity.x * correction + direction.x * (1 - correction);
        direction.y = myRigidbody.velocity.y * correction + direction.y * (1 - correction);
        direction.Normalize();
        direction.x *= moveSpeed;
        direction.y *= moveSpeed;
        myRigidbody.velocity = direction;
    }

    /// <summary>
    /// Plays the disappear animation and then self-destructs.
    /// </summary>
    public IEnumerator SelfDestroy()
    {
        animator.SetTrigger("Disappear");
        Destroy(gameObject, spellDeathTime);
        yield return null;
    }

    /// <summary>
    /// Deals damage to enemies that collide with the tornado.
    /// </summary>
    /// <param name="other">The collider of the object that the spell collides with.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
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
}
