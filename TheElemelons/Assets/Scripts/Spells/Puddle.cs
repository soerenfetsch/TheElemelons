using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Puddle class inherits from Spell Effect
/// Puddle spawned by Puddle Spell, slows enemies that step into puddle
/// </summary>
public class Puddle : SpellEffect
{
    public float spellLifeTime = 15f;
    public float slownessTime = 1f;
    public float slownessfactor = 3f;
    public float clockIntervalLength = 0.2f;
    public BoxCollider2D boxCollider2D;
    public float bigPuddleScale = 1.5f;
    public float bigPuddleProbability = 0.7f;
    /// <summary>
    /// initialize spelltype (water earth), start coroutine to expire and on off clock
    /// </summary>
    void Start()
    {
        if (Random.value < bigPuddleProbability)
        {
            gameObject.transform.localScale = new Vector3(bigPuddleScale, bigPuddleScale, 1);
        }
        spellType = new ChargeTuple(ChargeTypes.water, ChargeTypes.earth);
        StartCoroutine(OnOffClock());
        StartCoroutine(Expire());
    }
    /// <summary>
    /// destroy gameobject after spelllifetime expired
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
    /// <summary>
    /// clock to turn collider on and off so that enemies that are still standing on puddle get affected even tho they havent reentered
    /// </summary>
    IEnumerator OnOffClock()
    {
        while (true)
        {
            yield return new WaitForSeconds(clockIntervalLength);
            boxCollider2D.enabled = false;
            yield return new WaitForSeconds(clockIntervalLength);
            boxCollider2D.enabled = true;
        }
    }
    /// <summary>
    /// enemies entering puddle receive non stackable slowness debuff
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            GameObject target = hit.gameObject;
            if (hit != null)
            {
                BuffDebuffManager.Instance.ApplyBuffDebuff(target, "PuddleSpell", BuffDebuffManager.BuffDebuffType.SlownessDebuff, slownessTime, slownessfactor, false);
            }
        }
    }
}
