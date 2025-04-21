using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikiHead : MonoBehaviour
{
    Transform centerTransform;
    ElementType elementType;
    public float baseDamage = 0.5f;
    public float spinningDamage = 2f;
    public float thrust = 2f;
    public float knockTime = 0.3f;
    Tiki tikiScript;
    GameObject tiki;
    void Start()
    {
        tiki = transform.parent.parent.gameObject;
        tikiScript = tiki.GetComponent<Tiki>();
        centerTransform = transform.parent.transform;
        elementType = tikiScript.elementType;
    }
    /// <summary>
    /// deal damage to player if collides and player isnt staggered already
    /// damage increased if head was executing the boomerang attack
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
        other.isTrigger == true &&
        other.gameObject.GetComponent<Player>().GetCurrentState() != PlayerState.Stagger)
        {
            float damage = baseDamage;
            if (tikiScript.IsHeadSpinning() && !tikiScript.IsHeadAttached() && !tikiScript.IsTargetReached())
            {
                damage = spinningDamage;
            }
            Vector2 attackDirection = other.transform.position - centerTransform.position;
            CombatManager.ExecuteAttack(tiki, other.GetComponent<Rigidbody2D>(), elementType, attackDirection, damage, thrust, knockTime);
        }
    }
}
