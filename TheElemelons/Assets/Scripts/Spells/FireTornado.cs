using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// FireTornado class inherits from Spell Effect
/// FireTornado first moves in casting direction and can randomly change direction or bounce off walls
/// </summary>
public class FireTornado : SpellEffect
{
    [Header("Movement")]
    public float spellLifeTime = 3f;
    public float spellDeathTime = 0.3f;
    public float moveSpeed = 1f;
    /// <summary>
    /// direction correction factor if direction is changed
    /// </summary>
    public float correction = 0.1f;
    /// <summary>
    /// time between random direction switch
    /// </summary>
    public float switchTime = 1f;
    /// <summary>
    /// constraints of time between random direction switch
    /// </summary>
    public Vector2 switchTimeConstraints = new Vector2(1f, 3f);
    [Header("Hit")]
    public float thrust = 7f;
    public float knockTime = 0.4f;
    public float damage = 2f;
    [Header("Linked Components")]
    public Rigidbody2D myRigidbody;
    Transform castPoint;

    [Header("Internal Stuff")]
    /// <summary>
    /// current movement direction
    /// </summary>
    Vector2 direction;
    Animator animator;
    /// <summary>
    /// set spelltype (fire wind), initial direction
    /// start expiration timer
    /// start random direction switching coroutine
    /// </summary>
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
    /// <summary>
    /// random direction switching coroutine
    /// </summary>
    IEnumerator SwitchDirection(){
        while(true){
            yield return new WaitForSeconds(switchTime);
            switchTime = Random.Range(switchTimeConstraints.x,switchTimeConstraints.y);
            direction.x = Random.Range(-1,1);
            direction.y = Random.Range(-1,1);
            direction.Normalize();
        }
    }
    /// <summary>
    /// start expiration timer and self destruct at the end
    /// </summary>
    private IEnumerator Expire(){
        yield return new WaitForSeconds(spellLifeTime);
        StartCoroutine(SelfDestroy());
    }
    /// <summary>
    /// add force in current direction 
    /// </summary>
    void FixedUpdate()
    {   
        direction.x = myRigidbody.velocity.x*correction+direction.x*(1-correction);
        direction.y = myRigidbody.velocity.y*correction+direction.y*(1-correction);
        direction.Normalize();
        myRigidbody.AddForce(direction*moveSpeed,ForceMode2D.Impulse);
    }
    /// <summary>
    /// expire animation and then self destruct
    /// </summary>
    public IEnumerator SelfDestroy(){
        animator.SetTrigger("Disappear");
        Destroy(gameObject,spellDeathTime);
        yield return null;
    }
    /// <summary>
    /// deal damage to enimies that collide with tornado
    /// </summary>
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
