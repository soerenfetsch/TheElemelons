using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableMaxHeart : Collectable
{

    // Start is called before the first frame update
    void Start()
    {
        InitializeGeneral();
        playerHealth = player.GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }
    protected override void OnCollect()
    {
        playerHealth.SetMaxHealth(playerHealth.GetMaxHealth() + 1);
        playerHealth.SetHealth(playerHealth.GetMaxHealth());
        StartCoroutine(Expire());
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(shadow.gameObject);
        Destroy(gameObject);
    }
}
