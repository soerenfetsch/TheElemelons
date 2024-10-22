using System.Collections;
using System.Collections.Generic;
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
    Vector2 movement;
    Vector2 mousePos; 
    public Camera cam; 
    public PlayerState currentState;
    void Start(){
        currentState = PlayerState.walk;
        animator.SetFloat("Horizontal",0);
        animator.SetFloat("Vertical",-1);
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
        if (movement!=Vector2.zero)
        {
            movement.Normalize();
                rb.MovePosition(rb.position+movement*moveSpeed*Time.fixedDeltaTime);
            animator.SetFloat("Horizontal",movement.x);
            animator.SetFloat("Vertical",movement.y);
            
            wand_animator.SetFloat("Horizontal",movement.x);
            wand_animator.SetFloat("Vertical",movement.y);
            
        }
        //Aiming
        Vector2 aim_dir = mousePos-rb.position;
        float angle = Mathf.Atan2(aim_dir.y,aim_dir.x)*Mathf.Rad2Deg -90f;
        
        

    }

    public void Knock(float knockTime){
        StartCoroutine(KnockCo(knockTime));
    }
    private IEnumerator KnockCo(float knockTime){
        if (rb !=null )
        {
            yield return new WaitForSeconds(knockTime);
            rb.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            rb.velocity = Vector2.zero;
        }
    }
}
