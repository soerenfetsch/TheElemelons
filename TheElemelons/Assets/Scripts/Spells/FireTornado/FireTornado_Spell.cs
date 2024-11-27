using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTornado_Spell : SpellEffect
{
    [Header("Movement")]
    public float spellLifeTime = 3f;
    public float spellDeathTime = 0.3f;
    public float moveSpeed = 1f;
    public float aggroRange;
    [Header("Hit")]
    public float thrust = 7f;
    public float knockTime = 0.4f;
    public float damage = 2f;
    [Header("Linked Components")]
    Rigidbody2D myRigidbody;
    Transform castPoint;

    [Header("Target")]
    Transform target;
    [Header("Internal Stuff")]
    Vector2 direction;
    public float correctionFactor = 0.3f;
    Animator animator;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        spellType = new ChargeTuple(ChargeTypes.fire,ChargeTypes.wind);
        aggroRange = transform.GetComponentInChildren<CircleCollider2D>().radius;
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        direction = castPoint.transform.up;
        target = transform;
        StartCoroutine(Expire());

    }
    private IEnumerator Expire(){
        yield return new WaitForSeconds(spellLifeTime);
        StartCoroutine(SelfDestroy());
    }
    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, target.position)>aggroRange)
        {
            target = transform;
        }
        if(target!=transform){
            direction.x = direction.x - (transform.position.x - target.position.x) * correctionFactor;
            direction.y = direction.y - (transform.position.y - target.position.y) * correctionFactor;
            direction.Normalize();
        }
        myRigidbody.AddForce(direction*moveSpeed,ForceMode2D.Impulse);
    }
    public void setTarget(Transform newTarget){
        if (target==transform)
        {
            target = newTarget;
        }
    }
    
    public IEnumerator SelfDestroy(){
        animator.SetTrigger("Disappear");
        Destroy(gameObject,spellDeathTime);
        yield return null;
    }
}
