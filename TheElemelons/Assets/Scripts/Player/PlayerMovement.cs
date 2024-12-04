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
public class PlayerMovement : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float speedMultiplier = 1f;
    public float moveSpeed;
    
    [Header("Hurtbox")]
    public BoxCollider2D hurtbox;
    

    [Header("Internal Stuff")]
    Rigidbody2D rb;
    Animator animator;
    Animator wand_animator;
    public UnityEngine.Vector2 movement;
    public UnityEngine.Vector2 mousePos; 
    
    public PlayerState currentState = PlayerState.idle;
    public bool iFrames;
    public float iFramesTime = 0.3f;
    Health health;
    

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        wand_animator = transform.GetChild(1).GetComponent<Animator>();
        movement = UnityEngine.Vector2.zero;
        mousePos = UnityEngine.Vector2.zero;
        currentState = PlayerState.idle;
        iFrames = false;
        health = GetComponent<Health>();
        animator.SetFloat("Horizontal",0);
        animator.SetFloat("Vertical",-1);
        moveSpeed = baseSpeed*speedMultiplier;
    }
    void OnInputMove()
    {
        //Input
        animator.SetFloat("Speed",movement.sqrMagnitude);    


        moveSpeed = baseSpeed*speedMultiplier;
        //Physics
        if (movement!=UnityEngine.Vector2.zero && currentState != PlayerState.stagger)
        {
            currentState = PlayerState.walk;
            movement.Normalize();
            rb.MovePosition(rb.position+movement*moveSpeed*Time.fixedDeltaTime);
            animator.SetFloat("Horizontal",movement.x);
            animator.SetFloat("Vertical",movement.y);
            
            wand_animator.SetFloat("Horizontal",movement.x);
            wand_animator.SetFloat("Vertical",movement.y);
            
        }else if (currentState != PlayerState.stagger && movement == UnityEngine.Vector2.zero)
        {
            currentState = PlayerState.idle;
        }
    }

    void FixedUpdate(){
        OnInputMove();
        //Aiming
        UnityEngine.Vector2 aim_dir = mousePos-rb.position;
        float angle = Mathf.Atan2(aim_dir.y,aim_dir.x)*Mathf.Rad2Deg -90f;
    }

    public void Knock(float knockTime, float damage, UnityEngine.Vector2 difference){
        StartCoroutine(OnKnockbackMoveCo(knockTime,difference));
        health.SubtractHealth(damage);
    }
    private IEnumerator OnKnockbackMoveCo(float knockTime, UnityEngine.Vector2 difference){
        rb.AddForce(difference, ForceMode2D.Impulse);
        currentState = PlayerState.stagger;

        animator.SetBool("Stagger",true);
        animator.SetTrigger("Stagger_trig");
        wand_animator.SetBool("RaiseWand",false);
        if (rb !=null )
        {
            iFrames = true;
            yield return new WaitForSeconds(knockTime);
            rb.velocity = UnityEngine.Vector2.zero;
            currentState = PlayerState.idle;
            animator.SetBool("Stagger",false);
            yield return new WaitForSeconds(iFramesTime);
            iFrames = false;
        }
    }
    
   

    
}

