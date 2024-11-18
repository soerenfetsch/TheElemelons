using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Target_Finder : MonoBehaviour
{
    FireTornado_Spell fireTornado_Spell;
    void Start()
    {
        fireTornado_Spell = transform.parent.GetComponent<FireTornado_Spell>();
        gameObject.GetComponent<CircleCollider2D>().radius = fireTornado_Spell.aggroRange;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")&& other.isTrigger)
        {
            fireTornado_Spell.setTarget(other.GetComponent<Transform>());
        }
    }
}
