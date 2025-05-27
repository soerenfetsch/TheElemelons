using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableEarthElemelon : Collectable
{
    public Action OnEarthElemelonCollected;

    // Start is called before the first frame update
    void Start()
    {
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
        StartCoroutine(Expire());
    }
    IEnumerator Expire()
    {
        animator.SetTrigger("pickUp");
        collected = true;
        boxCollider2D.enabled = false;
        spriteRenderer.sortingOrder = 100;
        Destroy(shadow.gameObject);
        yield return new WaitForSeconds(0.6f);
        OnEarthElemelonCollected?.Invoke();
        Destroy(gameObject);
    }
}

