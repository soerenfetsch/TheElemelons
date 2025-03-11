using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Dash inherits from SpellEffect
/// Player is propelled in direction he was facing for given time
/// cant input movement while dash is active
/// </summary>
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
    /// <summary>
    /// initialize spelltype (notype, wind), Start dodash coroutine
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerRigidBody = player.transform.GetComponent<Rigidbody2D>();
        inputManager = player.GetComponent<InputManager>();
        spellType = new ChargeTuple(ChargeTypes.notype, ChargeTypes.wind);
        dashSpeed = dashDistance / dashTime;
        animator = gameObject.GetComponent<Animator>();
        StartCoroutine(DoDash());
    }
    /// <summary>
    /// lock movement inputs for player, compute dash movement vector, pass values to dash animator, wait out dashtime
    /// then destroy gameobject
    /// </summary>
    IEnumerator DoDash()
    {   
        //playerMovement.SetMovement(new Vector2(0,0));
        inputManager.canMove = false;
        move = new Vector2(player.GetComponent<Animator>().GetFloat("Horizontal"), player.GetComponent<Animator>().GetFloat("Vertical"));
        animator.SetFloat("Horizontal", move.x);
        animator.SetFloat("Vertical", move.y);
        move.Normalize();
        Physics2D.IgnoreLayerCollision(3,8);
        yield return new WaitForSeconds(dashTime);
        Physics2D.IgnoreLayerCollision(3,8,false);
        inputManager.canMove = true;
        Destroy(gameObject);
    }
    /// <summary>
    /// move player in given direction (calculated in dodash) and given speed, update dash gameobject position for the animation
    /// </summary>
    void FixedUpdate()
    {
        playerRigidBody.MovePosition(playerRigidBody.position + move * dashSpeed / playerMovement.GetMoveSpeed() * Time.fixedDeltaTime);
        transform.position = new Vector2(playerTransform.position.x, playerTransform.position.y - 2);
    }
}
