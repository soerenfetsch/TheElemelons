using System.Collections;
using UnityEngine;

/// <summary>
/// Bomb Spell class inherits from Spell Effect
/// Bomb explodes after given time and deals damage to enemies in collider
/// </summary>
public class Bomb_Spell : SpellEffect
{
    [Header("Hit")]
    public float thrust = 7f;
    public float knockTime = 0.4f;
    public float damage = 2f;
    [Header("Animation")]
    /// <summary>
    /// time between each blinking animation
    /// </summary>
    public float blinkTime = 0.1f;
    public Animator blinkingAnimator;
    [Header("Explosion")]
    public BoxCollider2D boxCollider2D;
    /// <summary>
    /// duration of explosion
    /// </summary>
    public float explodeTime = 0.5f;
    /// <summary>
    /// scale of gameobject during explosion
    /// </summary>
    public Vector2 explosionScale;

    /// <summary>
    /// Set Spelltype (fire earth) and start Bomb timer for explosion
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.fire, ChargeTypes.earth);
        StartCoroutine(BlinkTime());
        //dimmed light half shadow
        transform.Find("Light/HalfShadowMask").gameObject.SetActive(false);
    }
    /// <summary>
    /// make bomb blink for set time, then trigger explosion
    /// </summary>
    public IEnumerator BlinkTime()
    {
        for (int blinked = 0; blinked < 4; blinked++)
        {
            blinkingAnimator.SetTrigger("blink");
            yield return new WaitForSeconds(blinkTime);
            blinkingAnimator.SetTrigger("blink");
        }
        StartCoroutine(Explode());
    }
    /// <summary>
    /// Trigger explosion: Update animator and activate collisions, destroy gameobject after explosiontime
    /// </summary>
    private IEnumerator Explode()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(1).gameObject.SetActive(true); //light
        blinkingAnimator.SetTrigger("explode");
        boxCollider2D.enabled = true;
        gameObject.transform.localScale = explosionScale;
        yield return new WaitForSeconds(explodeTime);
        Destroy(gameObject);
    }
    /// <summary>
    /// Registers collisions during explosion and deals damage to enemies that collide
    /// </summary>
    /// /// <param name="other">The collider of the object that the spell collides with.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Enemy") && other.isTrigger && other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                CombatManager.ExecuteAttack(caster, hit, spellType, difference, damage, thrust, knockTime);
            }
        }
    }
    public void TurnLightOff()
    {
        StartCoroutine(TurnLightOffCoroutine());
    }
    IEnumerator TurnLightOffCoroutine()
    {
        transform.Find("Light/HalfShadowMask").gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        transform.Find("Light").gameObject.SetActive(false);
    }
}
