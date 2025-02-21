using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireballState{
    creating,
    propagating,
    exploding
}
public class Fireball : SpellEffect
{
    [Header("Hit")]
    public float thrust = 7f;
    public float knockTime = 0.4f;
    public float damage = 2f;
    [Header("Movement")]
    public float focusTime = 0.6f;
    public float flyTime = 3f;
    public float explodeTime = 0.5f;
    public float moveSpeed = 1f;
    [Header("Linked Components")]
    public Rigidbody2D myRigidbody;
    public Animator animator;
    public Transform castPoint;
    public BoxCollider2D boxCollider2D;
    [Header("Internal Infos")]
    public FireballState currentState;
    // Start is called before the first frame update
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.fire,ChargeTypes.fire);
        currentState = FireballState.creating;
        castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        transform.rotation = castPoint.rotation;
        GetComponent<SpriteRenderer>().flipX = true;
        GetComponent<SpriteRenderer>().flipY = true;
        
        StartCoroutine(Expire());
        StartCoroutine(StartFocus());
    }
    private IEnumerator Expire(){
        yield return new WaitForSeconds(flyTime);
        if (currentState != FireballState.exploding)
        {
            StartCoroutine(Explode());
        }
    }
    public IEnumerator StartFocus(){
        yield return new WaitForSeconds(focusTime);
        currentState = FireballState.propagating;
        animator.SetTrigger("fly");
    }
    private IEnumerator Explode(){
        currentState = FireballState.exploding;
        myRigidbody.isKinematic = true;
        myRigidbody.velocity = new Vector3(0, 0, 0);
        animator.SetTrigger("explode");
        boxCollider2D.size = new Vector2(1.95f, 2.02f);
        yield return new WaitForSeconds(explodeTime);
        Destroy(gameObject);
    }
     // Update is called once per frame
    void FixedUpdate()
    {
        if(currentState == FireballState.propagating){
            myRigidbody.AddForce(transform.up*moveSpeed,ForceMode2D.Impulse);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((other.gameObject.CompareTag("Enemy") && other.isTrigger) || other.gameObject.CompareTag("Collidable")) && currentState == FireballState.propagating)
        {
            StartCoroutine(Explode());
        } 
        if (currentState == FireballState.exploding && other.gameObject.CompareTag("Enemy")&& other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
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
