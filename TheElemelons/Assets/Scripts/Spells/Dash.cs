using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : SpellEffect
{
    public float dashTime = 0.3f;
    public float dashDistance = 5f;
    float dashSpeed;
    PlayerMovement playerMovement;
    Rigidbody2D playerRigidBody;
    InputManager inputManager;
    Transform playerTransform;
    GameObject player;
    Animator animator;
    Vector2 move;
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerRigidBody = player.transform.GetComponent<Rigidbody2D>();
        inputManager = player.GetComponent<InputManager>();
        spellType = new ChargeTuple(ChargeTypes.notype, ChargeTypes.wind);
        dashSpeed = dashDistance/dashTime;
        animator = gameObject.GetComponent<Animator>();
        StartCoroutine(DoDash());
    }
    
    IEnumerator DoDash(){
        //playerMovement.SetMovement(new Vector2(0,0));
        inputManager.canMove = false;
        move = new Vector2(player.GetComponent<Animator>().GetFloat("Horizontal"),player.GetComponent<Animator>().GetFloat("Vertical"));
        animator.SetFloat("Horizontal",move.x);
        animator.SetFloat("Vertical",move.y);
        move.Normalize();
        yield return new WaitForSeconds(dashTime);
        inputManager.canMove = true;
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        playerRigidBody.MovePosition(playerRigidBody.position + move * dashSpeed/playerMovement.GetMoveSpeed() * Time.fixedDeltaTime);
        transform.position = new Vector2(playerTransform.position.x,playerTransform.position.y-2);
    }
}
