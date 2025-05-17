using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchBoulderCollider : MonoBehaviour
{
    WitchBoulder spellEffect;
    void Start()
    {
        spellEffect = transform.parent.GetComponent<WitchBoulder>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger &&
            other.gameObject.GetComponent<Player>().GetCurrentState() != PlayerState.Stagger)
        {
            Vector2 attackDirection = other.transform.position - transform.position;
            CombatManager.ExecuteAttack(spellEffect.caster, other.GetComponent<Rigidbody2D>(), spellEffect.spellType, attackDirection, spellEffect.baseDamage, spellEffect.thrust, spellEffect.knockTime);
        }
    }
}
