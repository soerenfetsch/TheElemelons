using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Collider : MonoBehaviour
{
    float thrust;
    float knockTime;
    float damage;
    ChargeTypes spellType;
    FireTornado_Spell fireTornado_Spell;
    Transform tornadoTransform;
    // Start is called before the first frame update
    void Start()
    {
        fireTornado_Spell = transform.parent.GetComponent<FireTornado_Spell>();
        tornadoTransform = transform.parent.transform;
        thrust = fireTornado_Spell.thrust;
        damage = fireTornado_Spell.damage;
        knockTime = fireTornado_Spell.knockTime;
        spellType = fireTornado_Spell.spellType;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")&& other.isTrigger && other.GetComponent<Enemy>().currentState != EnemyState.stagger)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                other.GetComponent<Enemy>().Knock(hit, knockTime, damage,spellType);
                StartCoroutine(fireTornado_Spell.SelfDestroy());
            }
        }
    }
}
