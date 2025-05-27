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
    public float gravityStrength = 50f;
    Rigidbody2D playerRb;
    float baseDamage = 0.1f;
    float knockTime = 0f;
    float thrust = 0f;
    float spellLifeTime = 7f;
    float range = 10f;
    float force = 5f;
    SpriteRenderer spriteRenderer;
    Dictionary<GameObject, Rigidbody2D> victims;
    public GameObject redsparklesObject;
    List<GameObject> redSparkles;
    GameObject witch;
    new Rigidbody2D rigidbody2D;
    Vector3 startTarget;
    Vector3 startPoint;
    float moveSpeed = 3f;
    float driftAngle;
    CircleCollider2D circleCollider2D;
    // Start is called before the first frame update
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
    IEnumerator ColliderOnOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            circleCollider2D.enabled = !circleCollider2D.enabled;
        }
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        DoMovement();
        if (Time.frameCount % 31 == 0)
        {
            CatchVictims();
        }
        if (Time.frameCount % 60 == 0 && UnityEngine.Random.Range(0, 1) < 0.005f)
        {
            float distQuotient = 3f;
            CombatManager.InstantiateSpell(redsparklesObject, caster, transform.position + new Vector3(UnityEngine.Random.Range(-range / distQuotient, range / distQuotient), UnityEngine.Random.Range(-range / distQuotient, range / distQuotient)), Quaternion.Euler(0, 0, 0));
        }
        MoveVictims();
    }
    void DoMovement()
    {
        Vector2 dirRad = startTarget - startPoint;
        Vector2 move_by = (dirRad + (Vector2)(Quaternion.Euler(0f, 0f, driftAngle) * (Vector3)dirRad)).normalized * moveSpeed * Time.fixedDeltaTime;
        rigidbody2D.MovePosition(rigidbody2D.position + move_by);
    }
    void MoveVictims()
    {
        foreach ((GameObject key, Rigidbody2D rb) in victims)
        {
            Vector2 difference = (Vector2)transform.position - rb.position;
            rb.AddForce(difference.normalized * (range - difference.magnitude) * force);
        }
    }
    /// <summary>
    /// updates victims. adds new ones in range and removes those that are out of range.
    /// </summary>
    void CatchVictims()
    {
        List<GameObject> nextvictims = new List<GameObject>();
        LayerMask victimMask = LayerMask.GetMask("Enemies", "Player");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, victimMask);
        // add new 
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
        // remove those that got away
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
