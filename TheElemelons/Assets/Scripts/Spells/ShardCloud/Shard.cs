using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UIElements;

public class Shard : MonoBehaviour
{
    Transform pivot;
    public ShardCloud shardCloud;
    float radius;
    float angle = 0f;
    float rotationSpeed;
    Animator animator;
    public float deathTime = 0.4f; 
    Vector3 distance;
    Rigidbody2D myRigidBody;
    public float spellLifeTime;
    bool dying = false;

    float thrust;
    float damage;
    float knockTime;
    ChargeTypes spellType;
    void Start()
    {
        pivot = GameObject.Find("Player").transform;
        animator = gameObject.GetComponent<Animator>();
        radius = shardCloud.radius;
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        rotationSpeed = shardCloud.frequency*360;
        distance = transform.position - pivot.position;
        thrust = shardCloud.thrust;
        damage = shardCloud.damage;
        knockTime = shardCloud.knockTime;
        spellType = shardCloud.spellType;
        StartCoroutine(TimedDestruction());
    }
    
    void Update()
    {
        if (!dying)
        {
            transform.position = pivot.position + distance;
            transform.RotateAround(pivot.position,Vector3.forward,rotationSpeed*Time.deltaTime);
            distance = (transform.position - pivot.position).normalized * radius;   
        }
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!dying && other.gameObject.CompareTag("Enemy")&& other.isTrigger && other.GetComponent<Enemy>().currentState != EnemyState.stagger)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                other.GetComponent<Enemy>().Knock(hit, knockTime, damage,spellType);
                DieAnimation();
            }
        }
    }
    private IEnumerator TimedDestruction(){
        while (!(spellLifeTime > 0))
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(spellLifeTime);
        DieAnimation();
    }
    public void DieAnimation() 
    {
        dying = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        animator.SetBool("Hit",true);
        Debug.Log(animator.GetBool("Hit"));
    }
    public void SelfDestruct(){
        Destroy(gameObject);
    }
}
