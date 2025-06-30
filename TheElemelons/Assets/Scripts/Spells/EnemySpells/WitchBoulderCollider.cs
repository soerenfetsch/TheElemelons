using UnityEngine;

/// <summary>
/// Collider component for the WitchBoulder spell.
/// Handles collision detection with the player and applies damage and knockback.
/// </summary>
public class WitchBoulderCollider : MonoBehaviour
{
    // Reference to the parent WitchBoulder spell effect.
    WitchBoulder spellEffect;

    /// <summary>
    /// Caches the parent WitchBoulder component.
    /// </summary>
    void Start()
    {
        spellEffect = transform.parent.GetComponent<WitchBoulder>();
    }

    /// <summary>
    /// Handles collision with the player, applying damage and knockback if not staggered.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger &&
            other.gameObject.GetComponent<Player>().GetCurrentState() != PlayerState.Stagger)
        {
            Vector2 attackDirection = other.transform.position - transform.position;
            CombatManager.ExecuteAttack(
                spellEffect.caster,
                other.GetComponent<Rigidbody2D>(),
                spellEffect.spellType,
                attackDirection,
                spellEffect.baseDamage,
                spellEffect.thrust,
                spellEffect.knockTime
            );
        }
    }
}
