using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DefenseBuffSpell : SpellEffect
{
    public float defenseTime = 15f;
    public float defenseFactor = 2f;
    GameObject player;
    Transform playerTransform;
    InputManager inputManager;
    PlayerMovement playerMovement;
    Animator playerAnimator;
    Rigidbody2D playerRigidBody;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();
        playerRigidBody = player.transform.GetComponent<Rigidbody2D>();
        
        inputManager = player.GetComponent<InputManager>();
        BuffDebuffManager.Instance.ApplyBuffDebuff(player, BuffDebuffManager.BuffDebuffType.DefenseBuff, defenseTime, defenseFactor);
        StartCoroutine(Expire());
    }
    void FixedUpdate()
    {
        gameObject.transform.position = playerTransform.position;
    }
    IEnumerator Expire()
    {
        // Lock movement for animation
        playerMovement.SetMovement(new Vector2(0,0));
        playerAnimator.SetFloat("Vertical",-1);
        playerAnimator.SetFloat("Horizontal",0);
        playerRigidBody.velocity = new Vector2(0,0);
        inputManager.canInput = false;
        yield return new WaitForSeconds(1.5f);
        inputManager.canInput = true;

        yield return new WaitForSeconds(defenseTime);
        gameObject.GetComponent<Animator>().SetTrigger("Shatter");
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }
}
