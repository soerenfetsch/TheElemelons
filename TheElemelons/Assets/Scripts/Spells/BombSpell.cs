using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;

public class Bomb_Spell : SpellEffect
{
    [Header("Hit")]
    public float thrust = 7f;
    public float knockTime = 0.4f;
    public float damage = 2f;
    [Header("Animation")]
    public float blinkTime = 0.1f;
    public Animator blinkingAnimator;
    [Header("Explosion")]
    public BoxCollider2D boxCollider2D;
    public float explodeTime = 0.5f;
    public Vector2 explosionScale;
    
    // Start is called before the first frame update
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.fire,ChargeTypes.earth);
        StartCoroutine(BlinkTime());
    }
    public IEnumerator BlinkTime(){
        for (int blinked = 0; blinked < 4; blinked++)
        {
            blinkingAnimator.SetTrigger("blink");
            yield return new WaitForSeconds(blinkTime);
            blinkingAnimator.SetTrigger("blink");
        }
        StartCoroutine(Explode());
    }
    private IEnumerator Explode(){
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        blinkingAnimator.SetTrigger("explode");
        boxCollider2D.enabled = true;
        gameObject.transform.localScale = explosionScale;
        yield return new WaitForSeconds(explodeTime);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Enemy")&& other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                CombatManager.ExecuteAttack(caster, hit, spellType, difference, damage, thrust, knockTime);
            }
        }
    }
}
