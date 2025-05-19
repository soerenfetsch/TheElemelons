using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableHeart : Collectable
{
    float healValue = 1f;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger)
        {
            playerHealth.AddHealth(healValue);
            StartCoroutine(Expire());
        }
    }
    IEnumerator Expire()
    {
        animator.SetTrigger("heal");
        collected = true;
        boxCollider2D.enabled = false;
        spriteRenderer.sortingOrder = 100;
        Destroy(shadow.gameObject);
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
