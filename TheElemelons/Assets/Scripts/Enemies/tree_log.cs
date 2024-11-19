using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree_log : Enemy {

    
    public Rigidbody2D myRigidbody;
    [Header("Target Variables")]
    public Transform target;
    public float chaseRadius;
    public float attackRadius = 0f;

    [Header("Animator")]
    public Animator anim;

    [Header("Give Knockback to Player")]
    public float knockTime = 0.3f;
    public float thrust = 0.3f;
    public float damage = 0.25f;

    [Header("Hurtbox")]
    public BoxCollider2D hurtbox;
	// Use this for initialization
	void Start () {
        currentState = EnemyState.idle;
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        homePosition = transform.position;
        type = ChargeTypes.earth;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        CheckDistance();
	}

    public virtual void CheckDistance()
    {
        if(Vector3.Distance(target.position, 
                            transform.position) <= chaseRadius
           && Vector3.Distance(target.position,
                               transform.position) > attackRadius
            && currentState != EnemyState.stagger)
        {
            if (currentState == EnemyState.idle || currentState == EnemyState.walk)
            {
                Vector3 temp = Vector3.MoveTowards(transform.position,
                                                         target.position,
                                                         moveSpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                myRigidbody.MovePosition(temp);
                ChangeState(EnemyState.walk);
                anim.SetBool("wakeUp", true);
            }
        }else if (Vector3.Distance(target.position,
                            transform.position) > chaseRadius)
        {
            if (transform.position == new Vector3(homePosition.x,homePosition.y,0))
            {
                anim.SetBool("wakeUp", false);
            }else{
                Vector3 temp = Vector3.MoveTowards(transform.position,
                                                         homePosition,
                                                         moveSpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                myRigidbody.MovePosition(temp);
            }
        }
    }

    public void SetAnimFloat(Vector2 setVector){
        anim.SetFloat("Horizontal", setVector.x);
        anim.SetFloat("Vertical", setVector.y);
    }

    public void changeAnim(Vector2 direction){
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if(direction.x > 0){
                anim.SetFloat("Vertical",0f);
                anim.SetFloat("Horizontal",1f);
            }else if (direction.x < 0)
            {
                anim.SetFloat("Vertical",0f);
                anim.SetFloat("Horizontal",-1f);
            }
        }else if(Mathf.Abs(direction.x) < Mathf.Abs(direction.y)){
            if(direction.y > 0)
            {
                anim.SetFloat("Vertical",1f);
                anim.SetFloat("Horizontal",0f);
            }
            else if (direction.y < 0)
            {
                anim.SetFloat("Vertical",-1f);
                anim.SetFloat("Horizontal",0f);
            }
        }
    }

    public void ChangeState(EnemyState newState){
        if(currentState != newState)
        {
            currentState = newState;
        }
    }

    //deal damage to player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.isTrigger && 
            other.GetComponent<player_movement>().currentState != PlayerState.stagger )
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null && !hit.GetComponent<player_movement>().iFrames)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                
                hit.GetComponent<player_movement>().currentState = PlayerState.stagger;
                other.GetComponent<player_movement>().Knock(knockTime, baseAttack);
                myRigidbody.AddForce(-difference/2, ForceMode2D.Impulse);
                Knock(myRigidbody,0.1f,0,ChargeTypes.notype);
            }
        }
    }
}