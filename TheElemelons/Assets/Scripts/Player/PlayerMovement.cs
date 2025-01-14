using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float speedMultiplier = 1f;

    [Header("Hurtbox")]
    public BoxCollider2D hurtbox;


    [Header("Internal Stuff")]
    Player player;
    Rigidbody2D rb;
    Animator animator;
    Animator wandAnimator;
    UnityEngine.Vector2 movement;

    void Start()
    {
        Initialize();
    }

    //Initialize Variables
    public void Initialize()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        wandAnimator = transform.Find("Wand_Animation").GetComponent<Animator>();
        SetMovement(UnityEngine.Vector2.zero);
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", -1);
    }

    /// <summary>
    /// move player character according to player input
    /// </summary>
    public void OnInputMove()
    {
        //movement vector determined by input manager
        animator.SetFloat("Speed", GetMovement().sqrMagnitude);

        //Physics
        if (GetMovement() != UnityEngine.Vector2.zero && player.GetCurrentState() != PlayerState.Stagger)
        {
            player.SetCurrentState(PlayerState.Walk);
            movement.Normalize();
            //moving rigid body according to speed, current position and movement vector (which is the direction)
            rb.MovePosition(rb.position + GetMovement() * GetMoveSpeed() * Time.fixedDeltaTime);
            //Update animators
            animator.SetFloat("Horizontal", GetMovement().x);
            animator.SetFloat("Vertical", GetMovement().y);

            wandAnimator.SetFloat("Horizontal", GetMovement().x);
            wandAnimator.SetFloat("Vertical", GetMovement().y);

        }
        else if (player.GetCurrentState() != PlayerState.Stagger && GetMovement() == UnityEngine.Vector2.zero)
        {
            player.SetCurrentState(PlayerState.Idle);
        }
    }

    void FixedUpdate()
    {
        OnInputMove();
    }

    public void SetMovement(UnityEngine.Vector2 newMovement)
    {
        movement = newMovement;
    }

    public UnityEngine.Vector2 GetMovement()
    {
        return movement;
    }

    public float GetMoveSpeed()
    {
        return baseSpeed * speedMultiplier;
    }

}

