using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffDebuff : MonoBehaviour
{
    player_movement player_Movement;
    Casting casting;
    public Animator playerAnimator;
    public Animator speedBuffAnimator;
    public Transform speedBuffTransform;
    // Start is called before the first frame update
    void Start()
    {
        player_Movement = GetComponent<player_movement>();
        casting = GetComponent<Casting>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update(){
        //SpeedBuff Animator
        if (speedBuffAnimator != null)
        {
            if (player_Movement.speedMultiplier>1)
            {
                Debug.Log(player_Movement.currentState.HumanName());
                Debug.Log(player_Movement.currentState == PlayerState.idle);
                Debug.Log(player_Movement.currentState == PlayerState.stagger);
                if(player_Movement.currentState!=PlayerState.idle && player_Movement.currentState!=PlayerState.stagger)
                {
                    speedBuffAnimator.SetBool("run",true);
                    speedBuffAnimator.SetFloat("Horizontal",playerAnimator.GetFloat("Horizontal"));
                    speedBuffAnimator.SetFloat("Vertical",playerAnimator.GetFloat("Vertical"));
                }else{
                    speedBuffAnimator.SetBool("run",false);
                }    
            }else{
                    speedBuffAnimator.SetBool("run",false);
            }
        }
    }
    public void speedBuff(float moveSpeedFactor, float spellLifeTime,GameObject speedBuffGameObject){
        speedBuffAnimator = speedBuffGameObject.GetComponent<Animator>();
        player_Movement.speedMultiplier = Mathf.Max(player_Movement.speedMultiplier, moveSpeedFactor);
        StartCoroutine(speedBuffTimer(spellLifeTime, speedBuffGameObject));
    }

    IEnumerator speedBuffTimer(float spellLifeTime, GameObject speedBuffGameObject){
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(speedBuffGameObject);
        player_Movement.speedMultiplier = 1;
    }
}
