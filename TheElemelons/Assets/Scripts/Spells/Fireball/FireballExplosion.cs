using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireballExplosion : MonoBehaviour
{
    
    [Header("Hit")]
    public float thrust = 7f;
    public float knockTime = 0.4f;
    public float damage = 2f;
    public ChargeTuple spellType;
    [Header("Internal Stuff")]
    bool allow = false;    
    public void Start(){
        spellType = new ChargeTuple(ChargeTypes.fire,ChargeTypes.fire);
        StartCoroutine(Allow());
    }
    private IEnumerator Allow(){
        yield return new WaitForSeconds(0.01f);
        allow = true;
    }
    public void SelfDestruct(){
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (allow && other.gameObject.CompareTag("Enemy")&& other.isTrigger && other.GetComponent<Enemy>().currentState != EnemyState.stagger)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                other.GetComponent<Enemy>().Knock(hit, knockTime, damage,spellType);
            }
        }
    }
}
