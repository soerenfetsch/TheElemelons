using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Fireball has 3 states:
/// creating: no movement
/// propagating: propagates in casting direction
/// exploding: fireball has collided and now explodes and deals damage to enemies in collider
/// </summary>
public enum FireballState
{
    creating,
    propagating,
    exploding
}
/// <summary>
/// Fireball class inherits from Spell Effect
/// Fireball explodes after given time or collision and deals damage to enemies in collider
/// </summary>
public class Fireball : SpellEffect
{
    [Header("Hit")]
    public float thrust = 7f;
    public float knockTime = 0.4f;
    public float damage = 2f;
    [Header("Movement")]
    /// <summary>
    /// time fireball spends in creating state
    /// </summary>
    public float focusTime = 0.6f;
    /// <summary>
    /// time fireball spends in propagating state
    /// </summary>
    public float flyTime = 3f;
    /// <summary>
    /// time fireball spends in exploding state
    /// </summary>
    public float explodeTime = 0.5f;
    /// <summary>
    /// propagation speed
    /// </summary>
    public float moveSpeed = 1f;
    [Header("Linked Components")]
    public Rigidbody2D myRigidbody;
    public Animator animator;
    public Transform castPoint;
    public BoxCollider2D boxCollider2D;
    [Header("Internal Infos")]
    /// <summary>
    /// current fireball state
    /// </summary>
    public FireballState currentState;
    /// <summary>
    /// spellstype (fire fire), currentstate and orientation of fireball are set
    /// expiration timer of fireball is set (explodes after flytime at latest)
    /// focus timer is started to get to propagating state
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
    /// explosion triggered after flytime at latest
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
    /// switch to propagating state after focustime
    /// </summary>
    public IEnumerator StartFocus()
    {
        yield return new WaitForSeconds(focusTime);
        currentState = FireballState.propagating;
        animator.SetTrigger("fly");
    }
    /// <summary>
    /// explosion: update state to exploding, set rigidbody to kinematic, 
    // velocity 0, adapt collider scale to explosion sprite and destrox gameobject after explosion time
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
    /// add force to fireball in casting direction while in propagating state
    /// </summary>
    void FixedUpdate()
    {
        if (currentState == FireballState.propagating)
        {
            myRigidbody.AddForce(transform.up * moveSpeed, ForceMode2D.Impulse);
        }
    }
    /// <summary>
    /// if fireball currently propagating, collision with enemy or anything else will trigger explosion
    /// if fireball is exploding, collision with enemy deals damage to enemy
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Spell") && ((other.gameObject.CompareTag("Enemy") && other.isTrigger) || !other.gameObject.CompareTag("Enemy")) && currentState == FireballState.propagating)
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
}
