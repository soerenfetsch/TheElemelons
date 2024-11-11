using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class Speed_Buff_Spell : SpellEffect
{
    public float spellLifeTime = 30f;
    public float moveSpeedFactor = 1.5f;
    GameObject player;
    Transform playerTransform;
    Animator playerAnimator;
    Animator animator;
    player_movement player_Movement;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        playerAnimator = player.GetComponent<Animator>();
        StartCoroutine(speedBoost());
        player_Movement = player.GetComponent<player_movement>();
        animator = gameObject.GetComponent<Animator>();
    }
    void FixedUpdate(){
        Debug.Log(player_Movement.currentState.HumanName());
        Debug.Log(animator.GetBool("run"));
        if(player_Movement.currentState!=PlayerState.idle && player_Movement.currentState!=PlayerState.stagger)
        {
            transform.position = playerTransform.position;
            animator.SetBool("run",true);
            animator.SetFloat("Horizontal",playerAnimator.GetFloat("Horizontal"));
            animator.SetFloat("Vertical",playerAnimator.GetFloat("Vertical"));
        }else{
            animator.SetBool("false",false);
        }    
    }

    IEnumerator speedBoost(){
        //TODO: Change this buff system so that player cant buff himself into oblivion
        player_Movement.moveSpeed *= moveSpeedFactor;
        yield return new WaitForSeconds(spellLifeTime);
        player_Movement.moveSpeed /= moveSpeedFactor;
        Destroy(gameObject);
    }
}
