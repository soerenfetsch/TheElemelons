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
    public float baseSpeed = 5f;
    public float speedMultiplier = 1f;
    public float moveSpeed;
    
    [Header("Hurtbox")]
    public BoxCollider2D hurtbox;
    

    [Header("Internal Stuff")]
    public Rigidbody2D rb;
    public Animator animator;
    public Animator wand_animator;
    UnityEngine.Vector2 movement;
    UnityEngine.Vector2 mousePos; 
    public Camera cam; 
    public PlayerState currentState;
    public bool iFrames;
    public float iFramesTime = 0.3f;
    public Health_UI health_UI;

    void Start(){
        currentState = PlayerState.idle;
        animator.SetFloat("Horizontal",0);
        animator.SetFloat("Vertical",-1);
        
        iFrames = false;
        moveSpeed = baseSpeed*speedMultiplier;
    }
    // Update is called once per frame
    void Update()
    {
        //Aiming
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition); 
        
        //is player walking or idle
        if (currentState != PlayerState.stagger && Mathf.Abs(movement.x) >0 || Mathf.Abs(movement.y) >0)
        {
            currentState = PlayerState.walk;
        }else if(currentState != PlayerState.stagger && movement.x==0 && movement.y==0)
        {
            currentState = PlayerState.idle;
        }
    }
    void OnInputMove()
    {
        //Input
        //Movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        animator.SetFloat("Speed",movement.sqrMagnitude);    


        moveSpeed = baseSpeed*speedMultiplier;
        //Physics
        if (movement!=UnityEngine.Vector2.zero && currentState != PlayerState.stagger)
        {
            movement.Normalize();
            rb.MovePosition(rb.position+movement*moveSpeed*Time.fixedDeltaTime);
            animator.SetFloat("Horizontal",movement.x);
            animator.SetFloat("Vertical",movement.y);
            
            wand_animator.SetFloat("Horizontal",movement.x);
            wand_animator.SetFloat("Vertical",movement.y);
            
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
        health_UI.SubtractHealth(damage);
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
