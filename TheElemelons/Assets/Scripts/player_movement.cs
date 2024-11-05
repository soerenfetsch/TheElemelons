using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using UnityEngine;
public enum PlayerState{
    walk,
    attack,
    interact,
    stagger,
    idle
    }
public class player_movement : MonoBehaviour
{
    
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    public Animator wand_animator;
    UnityEngine.Vector2 movement;
    UnityEngine.Vector2 mousePos; 
    public Camera cam; 
    public PlayerState currentState;
    public float maxhealth = 5;
    public float health;
    [Header("Hurtbox")]
    public UnityEngine.Vector2 size = new UnityEngine.Vector2(0.79932f,1.54092f);
    public UnityEngine.Vector2 offset = new UnityEngine.Vector2(-0.0558f,-1.0541f);

    [Header("Death Effects")]
    public GameObject Death_Effect;
    private float deathEffectDelay = 1f;
    void Start(){
        currentState = PlayerState.walk;
        animator.SetFloat("Horizontal",0);
        animator.SetFloat("Vertical",-1);

        health = maxhealth;

        //player hurtbox
        BoxCollider2D sc = gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        sc.size = size;
        sc.offset = offset;
        sc.isTrigger = true;
    }
    // Update is called once per frame
    void Update()
    {
        //Input
        //Movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        animator.SetFloat("Speed",movement.sqrMagnitude);    
        
        //Aiming
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition); 

        //Raise Wand
        if (Input.GetMouseButtonDown(0) && !wand_animator.GetBool("RaiseWand"))
        {
            wand_animator.SetBool("RaiseWand",true);
        } 
        if (Input.GetMouseButtonUp(0) && wand_animator.GetBool("RaiseWand"))
        {
            wand_animator.SetBool("RaiseWand",false);
        }
    }

    void FixedUpdate(){
        //Physics
        if (movement!=UnityEngine.Vector2.zero)
        {
            movement.Normalize();
                rb.MovePosition(rb.position+movement*moveSpeed*Time.fixedDeltaTime);
            animator.SetFloat("Horizontal",movement.x);
            animator.SetFloat("Vertical",movement.y);
            
            wand_animator.SetFloat("Horizontal",movement.x);
            wand_animator.SetFloat("Vertical",movement.y);
            
        }
        //Aiming
        UnityEngine.Vector2 aim_dir = mousePos-rb.position;
        float angle = Mathf.Atan2(aim_dir.y,aim_dir.x)*Mathf.Rad2Deg -90f;
        
        

    }

    public void Knock(float knockTime, float damage){
        
        StartCoroutine(KnockCo(knockTime));
        TakeDamage(damage);
    }
    private IEnumerator KnockCo(float knockTime){
        if (rb !=null )
        {
            yield return new WaitForSeconds(knockTime);
            rb.velocity = UnityEngine.Vector2.zero;
            currentState = PlayerState.idle;
            rb.velocity = UnityEngine.Vector2.zero;
            currentState = PlayerState.walk;
        }
    }
    
    private void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            DeathEffect();

            this.gameObject.SetActive(false);
        }
    }

    private void DeathEffect()
    {
        if(Death_Effect != null)
        {
            GameObject effect = Instantiate(Death_Effect, transform.position, UnityEngine.Quaternion.identity);
            Destroy(effect, deathEffectDelay);
        }
    }
}
