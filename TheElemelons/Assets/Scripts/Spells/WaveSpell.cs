using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wave spell that travels forward, dealing damage and knockback to enemies and creating splash effects on contact.
/// The wave is blocked by walls and is destroyed after a short duration.
/// </summary>
public class WaveSpell : SpellEffect
{
    // Lifetime of the wave spell before it expires.
    float spellLifeTime = 3f;
    // Movement speed of the wave.
    float moveSpeed = 1f;
    // Direction of movement.
    Vector2 movement;
    // Rigidbody2D for moving the wave.
    new Rigidbody2D rigidbody2D;
    // Points from which to cast raycasts for wall detection.
    List<Transform> rayCastPoints;
    // Stores the first raycast hit (if any).
    RaycastHit2D hit;
    /// <summary>
    /// Prefab for the splash visual effect.
    /// </summary>
    public GameObject splashEffect;
    // Tracks which rigidbodies have splash effects attached.
    Dictionary<Rigidbody2D, GameObject> victims;
    // Damage dealt to enemies.
    float damage = 1f;
    // Knockback force applied to enemies.
    float thrust = 20f;
    // Duration of knockback.
    float knockTime = 0.7f;

    /// <summary>
    /// Initializes the wave's direction, rotation, raycast points, and starts the expiration timer.
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.water, ChargeTypes.water);
        movement = transform.position - caster.transform.position;
        Transform castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        transform.rotation = Quaternion.Euler(0, 0, 90) * castPoint.rotation;
        rigidbody2D = GetComponent<Rigidbody2D>();
        rayCastPoints = new List<Transform>();
        for (int i = 0; i < 3; i++)
        {
            rayCastPoints.Add(transform.GetChild(1).GetChild(i).transform);
        }
        victims = new Dictionary<Rigidbody2D, GameObject>();
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Moves the wave forward unless a wall is detected.
    /// </summary>
    void FixedUpdate()
    {
        if (!hit)
        {
            DoRaycasts();
            rigidbody2D.MovePosition(rigidbody2D.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Performs raycasts to prevent the wave from moving through walls.
    /// </summary>
    void DoRaycasts()
    {
        float rayLength = 0.8f;
        LayerMask mask = LayerMask.GetMask("Interactable", "Walls");
        foreach (Transform point in rayCastPoints)
        {
            if (!hit)
            {
                hit = Physics2D.Raycast(point.position, movement, rayLength, mask);
            }
        }
    }

    /// <summary>
    /// Waits for spellLifeTime, then triggers the shrink animation and destroys the wave.
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Shrink");
        yield return new WaitForSeconds(1.1f);
        Destroy(transform.gameObject);
    }

    /// <summary>
    /// Deals damage and knockback to enemies, and attaches splash effects to enemies and the player on contact.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger &&
            other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D otherRb = other.gameObject.GetComponent<Rigidbody2D>();
            if (!victims.ContainsKey(otherRb))
            {
                GameObject splash = Instantiate(splashEffect, other.transform.position + (Vector3)Vector3.down * 0.01f, Quaternion.Euler(0, 0, 0));
                splash.transform.localScale = other.transform.localScale;
                splash.transform.parent = other.transform;
                victims.Add(otherRb, splash);
            }
            CombatManager.ExecuteAttack(caster, other.GetComponent<Rigidbody2D>(), spellType, movement, damage, thrust, knockTime);
        }
        else if (other.gameObject.CompareTag("Player") && other.isTrigger)
        {
            Rigidbody2D otherRb = other.gameObject.GetComponent<Rigidbody2D>();
            if (!victims.ContainsKey(otherRb))
            {
                GameObject splash = Instantiate(splashEffect, other.transform.position + (Vector3)Vector3.down * 0.01f, Quaternion.Euler(0, 0, 0));
                splash.transform.localScale = other.transform.localScale;
                splash.transform.parent = other.transform;
                victims.Add(otherRb, splash);
            }
        }
    }

    /// <summary>
    /// Removes splash effect from enemies when they exit the wave.
    /// </summary>
    /// <param name="other">The collider that exited the trigger.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
        if (other.gameObject.CompareTag("Enemy") && otherRb != null && victims.ContainsKey(otherRb))
        {
            RemoveSplash(otherRb);
        }
    }

    /// <summary>
    /// Removes all splash effects when the wave is destroyed.
    /// </summary>
    void OnDestroy()
    {
        foreach ((Rigidbody2D key, GameObject value) in victims)
        {
            RemoveSplash(key, editList: false);
        }
    }

    // Removes the splash effect from a given rigidbody and resets its velocity.
    void RemoveSplash(Rigidbody2D key, bool editList = true)
    {
        Destroy(victims[key].gameObject);
        key.angularVelocity = 0;
        key.velocity = Vector2.zero;
        if (editList)
        {
            victims.Remove(key);
        }
    }
}
