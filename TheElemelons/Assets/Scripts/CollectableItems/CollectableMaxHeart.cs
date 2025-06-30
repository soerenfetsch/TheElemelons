using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableMaxHeart : Collectable
{

    // Start is called before the first frame update
    void Start()
    {
        shadowScale = 1.5f;
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }
    protected override void OnCollect()
    {
        if (playerHealth == null)
        {
            // When player health is not set up yet, wait for collectable to drop first
            return;
        }
        playerHealth.SetMaxHealth(playerHealth.GetMaxHealth() + 1);
        playerHealth.SetHealth(playerHealth.GetMaxHealth());
        StartCoroutine(Expire());
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
