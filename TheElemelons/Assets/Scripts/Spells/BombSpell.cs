using System.Collections;
using UnityEngine;

/// <summary>
/// Bomb Spell class inherits from SpellEffect.
/// Bomb explodes after a given time and deals damage to enemies in its collider.
/// </summary>
public class Bomb_Spell : SpellEffect
{
    [Header("Hit")]
    /// <summary>
    /// Force applied to targets when hit by the explosion.
    /// </summary>
    public float thrust = 7f;

    /// <summary>
    /// Duration of the knockback effect on targets.
    /// </summary>
    public float knockTime = 0.4f;

    /// <summary>
    /// Damage dealt by the bomb to targets.
    /// </summary>
    public float damage = 2f;

    [Header("Animation")]
    /// <summary>
    /// Time between each blinking animation before explosion.
    /// </summary>
    public float blinkTime = 0.1f;

    /// <summary>
    /// Animator for the blinking and explosion animation.
    /// </summary>
    public Animator blinkingAnimator;

    [Header("Explosion")]
    /// <summary>
    /// Collider used for the explosion hitbox.
    /// </summary>
    public BoxCollider2D boxCollider2D;

    /// <summary>
    /// Duration of the explosion effect.
    /// </summary>
    public float explodeTime = 0.5f;

    /// <summary>
    /// Scale of the GameObject during the explosion.
    /// </summary>
    public Vector2 explosionScale;

    /// <summary>
    /// Set spell type (fire earth) and start bomb timer for explosion.
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.fire, ChargeTypes.earth);
        StartCoroutine(BlinkTime());
        // Dimmed light half shadow
        transform.Find("Light/HalfShadowMask").gameObject.SetActive(false);
    }

    /// <summary>
    /// Make bomb blink for set time, then trigger explosion.
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
    /// Trigger explosion: update animator and activate collisions, destroy GameObject after explosion time.
    /// </summary>
    private IEnumerator Explode()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(1).gameObject.SetActive(true); // light
        blinkingAnimator.SetTrigger("explode");
        boxCollider2D.enabled = true;
        gameObject.transform.localScale = explosionScale;
        yield return new WaitForSeconds(explodeTime);
        Destroy(gameObject);
    }

    /// <summary>
    /// Registers collisions during explosion and deals damage to enemies that collide.
    /// </summary>
    /// <param name="other">The collider of the object that the spell collides with.</param>
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

    /// <summary>
    /// Turns off the bomb's light effect after a short delay.
    /// </summary>
    public void TurnLightOff()
    {
        StartCoroutine(TurnLightOffCoroutine());
    }

    // Coroutine to turn off the light effect.
    IEnumerator TurnLightOffCoroutine()
    {
        transform.Find("Light/HalfShadowMask").gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        transform.Find("Light").gameObject.SetActive(false);
    }
}
