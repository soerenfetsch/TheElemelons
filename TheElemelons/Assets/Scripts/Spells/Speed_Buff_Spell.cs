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
    Animator animator;
    BuffDebuff buffDebuff;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();
        animator = gameObject.GetComponent<Animator>();
        buffDebuff = player.GetComponent<BuffDebuff>();
        buffDebuff.speedBuff(moveSpeedFactor,spellLifeTime,gameObject);
    }
    void FixedUpdate(){
        transform.position = playerTransform.position;
    }
    
}
