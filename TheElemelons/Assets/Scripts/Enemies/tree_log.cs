using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree_log : Enemy {

    
    public Rigidbody2D myRigidbody;
    [Header("Target Variables")]
    public Transform target;
    public float chaseRadius;
    public float attackRadius;

    [Header("Animator")]
    public Animator anim;



	// Use this for initialization
	void Start () {
        currentState = EnemyState.idle;
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;

        //Hurtbox
        BoxCollider2D sc = gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        sc.size = new Vector2(1.182424f,1.504254f);
        sc.offset = new Vector2(-0.0402f,0.18755f);
        sc.isTrigger = true;

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
                               transform.position) > attackRadius)
        {
            if (currentState == EnemyState.idle || currentState == EnemyState.walk
                && currentState != EnemyState.stagger)
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
}