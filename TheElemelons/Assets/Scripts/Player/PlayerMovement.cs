using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;

//Handle movement of player character, including movement by player input and movement due to knockback
public enum PlayerState
{
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
    UnityEngine.Vector2 movement;
    public UnityEngine.Vector2 mousePos;
    PlayerState currentState;
    public bool iFrames;
    public float iFramesTime = 0.3f;
    Health health;
    void Start()
    {
        //Initialize Variables
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        wand_animator = transform.GetChild(1).GetComponent<Animator>();
        SetMovement(UnityEngine.Vector2.zero);
        mousePos = UnityEngine.Vector2.zero;
        SetCurrentState(PlayerState.idle);
        iFrames = false;
        health = GetComponent<Health>();
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", -1);
        moveSpeed = baseSpeed * speedMultiplier;
    }
    void OnInputMove()
    {
        //movement vector determined by input manager
        animator.SetFloat("Speed", GetMovement().sqrMagnitude);

        moveSpeed = baseSpeed * speedMultiplier;
        //Physics
        if (GetMovement() != UnityEngine.Vector2.zero && GetCurrentState() != PlayerState.stagger)
        {
            SetCurrentState(PlayerState.walk);
            movement.Normalize();
            //moving rigid body according to speed, current position and movement vector (which is the direction)
            rb.MovePosition(rb.position + GetMovement() * moveSpeed * Time.fixedDeltaTime);
            //Update animators
            animator.SetFloat("Horizontal", GetMovement().x);
            animator.SetFloat("Vertical", GetMovement().y);

            wand_animator.SetFloat("Horizontal", GetMovement().x);
            wand_animator.SetFloat("Vertical", GetMovement().y);

        }
        else if (GetCurrentState() != PlayerState.stagger && GetMovement() == UnityEngine.Vector2.zero)
        {
            SetCurrentState(PlayerState.idle);
        }
    }

    void FixedUpdate()
    {
        OnInputMove();
        //Update Aiming Arrow Direction
        UnityEngine.Vector2 aim_dir = mousePos - rb.position;
        float angle = Mathf.Atan2(aim_dir.y, aim_dir.x) * Mathf.Rad2Deg - 90f;
    }

    public void Knock(float knockTime, float damage, UnityEngine.Vector2 difference)
    {
        //apply knockback and damage to player
        StartCoroutine(OnKnockbackMoveCo(knockTime, difference));
        health.SubtractHealth(damage);
    }
    private IEnumerator OnKnockbackMoveCo(float knockTime, UnityEngine.Vector2 difference)
    {
        //move player according to input variables
        //Update Player State and Animator accordingly
        rb.AddForce(difference, ForceMode2D.Impulse);
        SetCurrentState(PlayerState.stagger);

        animator.SetBool("Stagger", true);
        animator.SetTrigger("Stagger_trig");
        wand_animator.SetBool("RaiseWand", false);
        if (rb != null)
        {
            iFrames = true;
            yield return new WaitForSeconds(knockTime);
            rb.velocity = UnityEngine.Vector2.zero;
            SetCurrentState(PlayerState.idle);
            animator.SetBool("Stagger", false);
            yield return new WaitForSeconds(iFramesTime);
            iFrames = false;
        }
    }
    public void SetCurrentState(PlayerState newState)
    {
        currentState = newState;
    }
    public PlayerState GetCurrentState()
    {
        return currentState;
    }
    public void SetMovement(UnityEngine.Vector2 newMovement)
    {
        movement = newMovement;
    }
    public UnityEngine.Vector2 GetMovement()
    {
        return movement;
    }



}

