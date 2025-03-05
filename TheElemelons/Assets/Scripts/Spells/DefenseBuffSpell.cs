using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DefenseBuffSpell : SpellEffect
{
    public float defenseTime = 15f;
    public float defenseFactor = 2f;
    public RuntimeAnimatorController earthPlayerAnimator;
    public RuntimeAnimatorController playerRuntimeAnimator;
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

        yield return new WaitForSeconds(defenseTime-1.5f);

        bool otherSpellActive = false;
        List<BuffDebuffManager.BuffDebuff> buffList = BuffDebuffManager.Instance.GetActiveEffects(player);
            foreach (BuffDebuffManager.BuffDebuff buff in buffList) {
            if (buff!=null && buff.type == BuffDebuffManager.BuffDebuffType.DefenseBuff){
                otherSpellActive = true;
            }
            }
        if (!otherSpellActive)
        {
            playerAnimator.runtimeAnimatorController = playerRuntimeAnimator;
            gameObject.GetComponent<Animator>().SetTrigger("Shatter");
            yield return new WaitForSeconds(0.4f);
        }
        
        Destroy(gameObject);
    }
    public void Switch_to_Earthy_Animator()
    {
        RuntimeAnimatorController animatorController = playerAnimator.GetComponent<RuntimeAnimatorController>();
        playerAnimator.runtimeAnimatorController =  earthPlayerAnimator;
        earthPlayerAnimator = animatorController;
    }
}
