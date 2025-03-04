using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WallSpell : SpellEffect
{
    public float spellLifeTime = 7f;
    public float distance = 3f;
    Transform playerTransform;
    BoxCollider2D boxCollider2D;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.earth, ChargeTypes.earth);
        playerTransform = GameObject.Find("Player").transform;
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        Orientate();
        StartCoroutine(Expire());
    }
    void Orientate()
    {
        Vector2 direction = transform.position - playerTransform.position;
        direction.Normalize();
        
        if (direction.x < -0.707 && direction.y > -0.707 && direction.y < 0.707)
        {
            //Left
            animator.SetInteger("Direction",-1);
            boxCollider2D.offset = new Vector2(0.01683933f,0.9102298f);
            boxCollider2D.size = new Vector2(0.5423633f,1.82007f);
        } else if (direction.x > 0.707 && direction.y > -0.707 && direction.y < 0.707)
        {
            //Right
            animator.SetInteger("Direction",1);
            boxCollider2D.offset = new Vector2(-0.09968555f,0.9684917f);
            boxCollider2D.size = new Vector2(0.513232f,1.878331f);
        }
        boxCollider2D.enabled = true;
        
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        animator.SetTrigger("Sink");
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
