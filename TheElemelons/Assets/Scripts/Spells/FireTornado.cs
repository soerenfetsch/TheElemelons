using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTornado : SpellEffect
{
    [Header("Movement")]
    public float spellLifeTime = 3f;
    public float spellDeathTime = 0.3f;
    public float moveSpeed = 1f;
    public float switchTime = 1f;
    public float correction = 0.1f;
    public Vector2 switchTimeConstraints = new Vector2(1f, 3f);
    [Header("Hit")]
    public float thrust = 7f;
    public float knockTime = 0.4f;
    public float damage = 2f;
    [Header("Linked Components")]
    public Rigidbody2D myRigidbody;
    Transform castPoint;

    [Header("Internal Stuff")]
    Vector2 direction;
    Animator animator;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        spellType = new ChargeTuple(ChargeTypes.fire,ChargeTypes.wind);
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        direction = castPoint.transform.up;
        StartCoroutine(Expire());
        StartCoroutine(SwitchDirection());

    }
    IEnumerator SwitchDirection(){
        while(true){
            yield return new WaitForSeconds(switchTime);
            switchTime = Random.Range(switchTimeConstraints.x,switchTimeConstraints.y);
            direction.x = Random.Range(-1,1);
            direction.y = Random.Range(-1,1);
            direction.Normalize();
        }
    }
    private IEnumerator Expire(){
        yield return new WaitForSeconds(spellLifeTime);
        StartCoroutine(SelfDestroy());
    }
    void FixedUpdate()
    {   
        direction.x = myRigidbody.velocity.x*correction+direction.x*(1-correction);
        direction.y = myRigidbody.velocity.y*correction+direction.y*(1-correction);
        direction.Normalize();
        myRigidbody.AddForce(direction*moveSpeed,ForceMode2D.Impulse);
    }
    
    public IEnumerator SelfDestroy(){
        animator.SetTrigger("Disappear");
        Destroy(gameObject,spellDeathTime);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")&& other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                CombatManager.ExecuteAttack(caster, hit, spellType, difference, damage, thrust, knockTime);
            }
        }
    }
}
