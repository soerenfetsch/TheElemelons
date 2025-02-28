using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puddle : SpellEffect
{
    public float slownessTime = 1f;
    public float slownessfactor = 0.3f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                GameObject target = hit.gameObject;
                BuffDebuffManager.Instance.ApplyBuffDebuff(target, BuffDebuffManager.BuffDebuffType.SpeedBuff, slownessTime, slownessfactor);
            }
        }
    }
}
