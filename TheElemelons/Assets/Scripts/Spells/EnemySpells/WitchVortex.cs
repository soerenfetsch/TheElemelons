using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vortex that moves in a curve away from its caster. Deals minor damage to player on collision (~0.1 hearts). 
/// Sucks player and enemies in that are in its range (~10 units). 
/// Force parameter controls strength. Also can randomly spawn red sparkles around itself.
/// </summary>
public class WitchVortex : SpellEffect
{
    /// <summary>
    /// Strength of the vortex's gravity pull.
    /// </summary>
    public float gravityStrength = 50f;

    // Reference to the player's Rigidbody2D.
    Rigidbody2D playerRb;

    // Damage dealt to the player on collision.
    float baseDamage = 0.1f;
    // Knockback duration for the player.
    float knockTime = 0f;
    // Knockback force for the player.
    float thrust = 0f;
    // Lifetime of the vortex spell.
    float spellLifeTime = 7f;
    // Range of the vortex's suction effect.
    float range = 10f;
    // Force applied to victims within the vortex.
    float force = 5f;
    // Reference to the vortex's SpriteRenderer.
    SpriteRenderer spriteRenderer;
    // Dictionary of currently affected victims and their rigidbodies.
    Dictionary<GameObject, Rigidbody2D> victims;
    /// <summary>
    /// Prefab for the red sparkles effect.
    /// </summary>
    public GameObject redsparklesObject;
    // List of spawned red sparkles.
    List<GameObject> redSparkles;
    // Reference to the Witch GameObject.
    GameObject witch;
    // Rigidbody2D for vortex movement.
    new Rigidbody2D rigidbody2D;
    // Target position for the vortex's movement.
    Vector3 startTarget;
    // Starting position of the vortex.
    Vector3 startPoint;
    // Movement speed of the vortex.
    float moveSpeed = 3f;
    // Angle for the vortex's drift movement.
    float driftAngle;
    // Reference to the vortex's collider.
    CircleCollider2D circleCollider2D;

    /// <summary>
    /// Initializes references, sets up movement, and starts coroutines.
    /// </summary>
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spellType = new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype);
        witch = GameObject.Find("Witch");
        victims = new Dictionary<GameObject, Rigidbody2D>();
        startTarget = GameObject.Find("Player").transform.position;
        driftAngle = UnityEngine.Random.Range(-45f, 45f);
        startPoint = transform.position;
        circleCollider2D = GetComponent<CircleCollider2D>();
        StartCoroutine(ColliderOnOff());
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Coroutine: Periodically toggles the collider on and off.
    /// </summary>
    IEnumerator ColliderOnOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            circleCollider2D.enabled = !circleCollider2D.enabled;
        }
    }

    /// <summary>
    /// Coroutine: Waits for spellLifeTime, then destroys the vortex GameObject.
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }

    /// <summary>
    /// Handles movement, victim suction, and random sparkle spawning.
    /// </summary>
    void FixedUpdate()
    {
        if (caster == null)
        {
            Destroy(gameObject);
        }
        DoMovement();
        if (Time.frameCount % 31 == 0)
        {
            CatchVictims();
        }
        if (Time.frameCount % 60 == 0 && UnityEngine.Random.Range(0, 1) < 0.005f)
        {
            float distQuotient = 3f;
            CombatManager.InstantiateSpell(
                redsparklesObject,
                caster,
                transform.position + new Vector3(
                    UnityEngine.Random.Range(-range / distQuotient, range / distQuotient),
                    UnityEngine.Random.Range(-range / distQuotient, range / distQuotient)
                ),
                Quaternion.Euler(0, 0, 0)
            );
        }
        MoveVictims();
    }

    // Moves the vortex in a curved path away from its caster.
    void DoMovement()
    {
        Vector2 dirRad = startTarget - startPoint;
        Vector2 move_by = (dirRad + (Vector2)(Quaternion.Euler(0f, 0f, driftAngle) * (Vector3)dirRad)).normalized * moveSpeed * Time.fixedDeltaTime;
        rigidbody2D.MovePosition(rigidbody2D.position + move_by);
    }

    // Applies suction force to all victims within the vortex's range.
    void MoveVictims()
    {
        foreach ((GameObject key, Rigidbody2D rb) in victims)
        {
            Vector2 difference = (Vector2)transform.position - rb.position;
            rb.AddForce(difference.normalized * (range - difference.magnitude) * force);
        }
    }

    /// <summary>
    /// Updates victims: adds new ones in range and removes those that are out of range.
    /// </summary>
    void CatchVictims()
    {
        List<GameObject> nextvictims = new List<GameObject>();
        LayerMask victimMask = LayerMask.GetMask("Enemies", "Player");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, victimMask);
        // Add new victims
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != witch)
            {
                nextvictims.Add(collider.gameObject);
                Rigidbody2D rb = collider.gameObject.GetComponent<Rigidbody2D>();
                if (!victims.ContainsKey(collider.gameObject) && rb != null)
                {
                    victims.Add(collider.gameObject, rb);
                }
            }
        }
        // Remove those that got away
        if (nextvictims.Count < victims.Count)
        {
            List<GameObject> keysToRemove = new List<GameObject>();
            foreach ((GameObject key, Rigidbody2D rb) in victims)
            {
                if (!nextvictims.Contains(key))
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0;
                    keysToRemove.Add(key);
                }
            }
            foreach (GameObject key in keysToRemove)
            {
                victims.Remove(key);
            }
        }
    }

    /// <summary>
    /// Handles collision with the player, applying minor damage and knockback if not staggered.
    /// </summary>
    /// <param name="other">The collider that entered the vortex.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger &&
            other.gameObject.GetComponent<Player>().GetCurrentState() != PlayerState.Stagger)
        {
            Vector2 attackDirection = other.transform.position - transform.position;
            CombatManager.ExecuteAttack(caster, other.GetComponent<Rigidbody2D>(), spellType, attackDirection, baseDamage, thrust, knockTime);
        }
    }
}
