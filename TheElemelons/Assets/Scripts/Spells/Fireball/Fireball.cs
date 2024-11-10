using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public enum FireballState{
    creating,
    propagating,
    exploding
}
public class Fireball : SpellEffect
{
    [Header("Movement")]
    public float spellLifeTime = 3f;
    public float moveSpeed = 1f;
    [Header("Linked Components")]
    public GameObject explosion;
    public Rigidbody2D myRigidbody;
    public Animator animator;
    public Transform castPoint;
    [Header("Internal Infos")]
    public FireballState currentState;
    
    // Start is called before the first frame update
    void Start()
    {
        currentState = FireballState.creating;
        castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        transform.rotation = castPoint.rotation;
        GetComponent<SpriteRenderer>().flipX = true;
        GetComponent<SpriteRenderer>().flipY = true;
        
        StartCoroutine(Focus());
    }
    private IEnumerator Focus(){
        Destroy(gameObject,spellLifeTime);
        yield return 1;
    }
    public void startFocus(){
        currentState = FireballState.propagating;
        animator.SetTrigger("fly");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(currentState == FireballState.propagating){
            myRigidbody.AddForce(transform.up*moveSpeed,ForceMode2D.Impulse);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Enemy")&& other.isTrigger) || other.gameObject.CompareTag("collidable") && currentState == FireballState.propagating)
        {
            Instantiate(explosion, transform.position,UnityEngine.Quaternion.Euler(0,0,0));
            Destroy(gameObject);
        }
    }
}
