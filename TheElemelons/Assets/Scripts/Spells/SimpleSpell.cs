using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a simple spell cast when no charge is selected.
/// Summons weak, short-lived sparkles that deal minor damage and knockback.
/// </summary>
public class SimpleSpell : SpellEffect
{
    /// <summary>
    /// Lifetime of the spell before it self-destructs.
    /// </summary>
    public float spellLifeTime = 1f;

    /// <summary>
    /// Force applied to targets when hit by the spell.
    /// </summary>
    public float thrust = 1f;

    /// <summary>
    /// Duration of the knockback effect on targets.
    /// </summary>
    public float knockTime = 0.2f;

    /// <summary>
    /// Damage dealt by the spell to targets.
    /// </summary>
    public float damage = 0f;

    /// <summary>
    /// Initializes the spell with a default type and starts the self-destruction coroutine.
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype);
        StartCoroutine(SelfDestruct());
    }

    /// <summary>
    /// Coroutine to destroy the spell after its lifetime expires.
    /// </summary>
    IEnumerator SelfDestruct()
    {
        Destroy(gameObject, spellLifeTime);
        yield return null;
    }

    /// <summary>
    /// Handles collision detection with other game objects.
    /// </summary>
    /// <param name="other">The collider of the object that the spell collides with.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Future code for handling collisions
        /*
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger && other.GetComponent<Enemy>().currentState != EnemyState.stagger)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                other.GetComponent<Enemy>().Knock(hit, knockTime, damage, spellType);
                Destroy(gameObject);
            }
        }
        */
    }
}
