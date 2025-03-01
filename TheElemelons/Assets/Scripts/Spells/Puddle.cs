using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Puddle : SpellEffect
{
    public float spellLifeTime = 15f;
    public float slownessTime = 1f;
    public float slownessfactor = 3f;
    public float clockIntervalLength = 0.2f;
    public BoxCollider2D boxCollider2D;
    public float bigPuddleScale = 1.5f;
    public float bigPuddleProbability = 0.7f;
    void Start()
    {
        if (Random.value < bigPuddleProbability)
        {
            gameObject.transform.localScale = new Vector3(bigPuddleScale,bigPuddleScale,1);
        }
        spellType = new ChargeTuple(ChargeTypes.water, ChargeTypes.earth);
        StartCoroutine(OnOffClock());
        StartCoroutine(Expire());
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            GameObject target = hit.gameObject;
            List<BuffDebuffManager.BuffDebuff> buffList = BuffDebuffManager.Instance.GetActiveEffects(target);
            int count=0;
            foreach (BuffDebuffManager.BuffDebuff buff in buffList) {
            if (buff!=null && buff.type == BuffDebuffManager.BuffDebuffType.SlownessDebuff) count++;
            }
            if (hit != null && count == 0)
            {
                BuffDebuffManager.Instance.ApplyBuffDebuff(target, BuffDebuffManager.BuffDebuffType.SlownessDebuff, slownessTime, slownessfactor);
            }
        }
    }
}
