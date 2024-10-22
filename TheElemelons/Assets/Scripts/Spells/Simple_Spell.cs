using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_Spell : MonoBehaviour
{
    public float spellLifeTime = 1f;
    public float thrust = 0.2f;
    public float knockTime = 0.2f;
    public float damage = 1f;
    public ChargeTypes spellType = ChargeTypes.notype;
    public GameObject effect;
    // Start is called before the first frame update
    void Start (){
        StartCoroutine(SelfDestruct());
    }
    void Update()
    {
        
    }
    IEnumerator SelfDestruct(){
        Destroy(effect,spellLifeTime);
        yield return 1;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                if (other.gameObject.CompareTag("Enemy") && other.isTrigger)
                {
                    hit.GetComponent<Enemy>().currentState = EnemyState.stagger;
                    other.GetComponent<Enemy>().Knock(hit, knockTime, damage,spellType);
                }
                
            }
        }
    }
}


