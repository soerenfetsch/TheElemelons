using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePinkPotion : Collectable
{
    public List<BuffDebuffManager.BuffDebuffType> buffDebuffchoices;
    public float factor = 1.5f;
    public float duration = 120f;

    void Start()
    {
        InitializeGeneral();
    }

    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Applies a random buff or debuff to the player and starts the pickup animation when collected.
    /// </summary>
    protected override void OnCollect()
    {
        BuffDebuffManager.Instance.ApplyBuffDebuff(playerTransform.gameObject, "PinkPotion", ChoseBuff(), duration, factor, true);
        StartCoroutine(Expire());
    }

    BuffDebuffManager.BuffDebuffType ChoseBuff()
    {
        int i = UnityEngine.Random.Range(0, buffDebuffchoices.Count);
        return buffDebuffchoices[i];
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
