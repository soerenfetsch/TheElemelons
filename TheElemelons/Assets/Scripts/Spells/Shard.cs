using System.Collections;
using UnityEngine;

/// <summary>
/// Shard class inherits from Spell Effect
/// Shard rotates around player and itself
/// Is spawned by a ShardCloud Spell Effect
/// breaks at impact with enemy or when lifetime exhausted
/// </summary>
public class Shard : SpellEffect
{
    [Header("Movement")]
    Transform pivot;
    float radius;
    float rotationSpeed;
    Vector3 distance;
    [Header("Death")]
    public float deathTime = 0.4f;
    Rigidbody2D myRigidBody;
    [Header("Internal Stuff")]
    public ShardCloud shardCloud;
    Animator animator;
    /// <summary>
    /// spelltype (wind earth) initialized, selfdestruct timer started
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.wind, ChargeTypes.earth);
        pivot = GameObject.Find("Player").transform;
        animator = gameObject.GetComponent<Animator>();
        radius = shardCloud.radius;
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        rotationSpeed = shardCloud.frequency * 360;
        distance = transform.position - pivot.position;
        caster = shardCloud.caster;
        StartCoroutine(TimedDestruction());
    }
    /// <summary>
    /// rotate around player at set distance
    /// </summary>
    void Update()
    {
        if (!animator.GetBool("Hit"))
        {
            transform.position = pivot.position + distance;
            transform.RotateAround(pivot.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            distance = (transform.position - pivot.position).normalized * radius;
        }

    }
    /// <summary>
    /// deal damage to enemy if collides and self destructed
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                DieAnimation();
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * shardCloud.thrust;
                CombatManager.ExecuteAttack(caster, hit, spellType, difference, shardCloud.damage, shardCloud.thrust, shardCloud.knockTime);
            }
        }
    }
    /// <summary>
    /// self destruct after spelllifetime exhausted
    /// </summary>
    private IEnumerator TimedDestruction()
    {
        while (!(shardCloud.spellLifeTime > 0))
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(shardCloud.spellLifeTime);
        DieAnimation();
    }
    /// <summary>
    /// trigger break animation and self destruct
    /// </summary>
    public void DieAnimation()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        animator.SetBool("Hit", true);
        Debug.Log(animator.GetBool("Hit"));
        StartCoroutine(SelfDestruct());
    }
    /// <summary>
    /// self destruct after break animation finished
    /// </summary>
    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}
