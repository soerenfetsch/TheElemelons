using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
/// <summary>
/// WallSpell inherits from SpellEffect
/// Spawns Wall for given time
/// </summary>
public class WallSpell : SpellEffect
{
    public float spellLifeTime = 7f;
    public float distance = 3f;
    Transform playerTransform;
    BoxCollider2D boxCollider2D;
    Animator animator;
    /// <summary>
    /// Initialize spelltype (earth, earth), call method to orientate wall and start expiration coroutine
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.earth, ChargeTypes.earth);
        playerTransform = GameObject.Find("Player").transform;
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        Orientate();
        StartCoroutine(Expire());
    }
    /// <summary>
    /// orientate wall in correct direction according to where player is and adapt the collider accordingly, as well as animations
    /// </summary>
    void Orientate()
    {
        Vector2 direction = transform.position - playerTransform.position;
        direction.Normalize();

        if (direction.x < -0.707 && direction.y > -0.707 && direction.y < 0.707)
        {
            //Left
            animator.SetInteger("Direction", -1);
            boxCollider2D.offset = new Vector2(0.01683933f, 0.9102298f);
            boxCollider2D.size = new Vector2(0.5423633f, 1.82007f);
        }
        else if (direction.x > 0.707 && direction.y > -0.707 && direction.y < 0.707)
        {
            //Right
            animator.SetInteger("Direction", 1);
            boxCollider2D.offset = new Vector2(-0.09968555f, 0.9684917f);
            boxCollider2D.size = new Vector2(0.513232f, 1.878331f);
        }
        boxCollider2D.enabled = true;

    }
    /// <summary>
    /// Trigger sink animation after spelllifetime and destroy gameobject
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        animator.SetTrigger("Sink");
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
