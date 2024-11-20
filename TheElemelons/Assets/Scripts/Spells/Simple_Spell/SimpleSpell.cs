using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpell : SpellEffect
{
    public float spellLifeTime = 1f;
    public float thrust = 1f;
    public float knockTime = 0.2f;
    public float damage = 0f;
    public float delay = 0.2f;
    bool delaydone = false;
    // Start is called before the first frame update
    void Start (){
        spellType = new ChargeTuple(ChargeTypes.notype,ChargeTypes.notype);
        StartCoroutine(Delay());
        StartCoroutine(SelfDestruct());
    }
    IEnumerator Delay(){
        yield return new WaitForSeconds(delay);
        delaydone = true;
    }
    IEnumerator SelfDestruct(){
        Destroy(gameObject,spellLifeTime);
        yield return 1;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (delaydone && other.gameObject.CompareTag("Enemy")&& other.isTrigger && other.GetComponent<Enemy>().currentState != EnemyState.stagger)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                other.GetComponent<Enemy>().Knock(hit, knockTime, damage,spellType);
                Destroy(gameObject);
            }
        }
    }
}


