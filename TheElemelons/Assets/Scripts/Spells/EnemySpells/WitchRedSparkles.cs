using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Red sparkle effect. GameObject is destroyed after spellLifeTime runs out (~2s). 
/// On collision with the player, applies either a weakness or fragility debuff (randomly chosen).
/// Each player collider is only affected once per sparkle.
/// </summary>
public class WitchRedSparkles : SpellEffect
{
    /// <summary>
    /// Duration of the debuff applied to the player.
    /// </summary>
    public float debuffTime = 10f;

    /// <summary>
    /// Factor by which the debuff affects the player.
    /// </summary>
    public float debuffFactor = 1.2f;

    /// <summary>
    /// Lifetime of the sparkle effect before it fades out and is destroyed.
    /// </summary>
    public float spellLifeTime = 2f;

    // List of colliders already affected by this sparkle (prevents duplicate debuffs).
    List<Collider2D> affecteds;

    // Reference to the SpriteRenderer for fading.
    SpriteRenderer spriteRenderer;

    /// <summary>
    /// Initializes references and starts the expiration coroutine.
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spellType = new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype);
        affecteds = new List<Collider2D>();
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Destroys the sparkle if the caster no longer exists.
    /// </summary>
    void FixedUpdate()
    {
        if (caster == null)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Waits for spellLifeTime, then fades out and destroys the GameObject.
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        float fadeTimer = 1f;
        float fadeDuration = fadeTimer;
        Color c = spriteRenderer.color;
        float chunkiness = 30f;
        while (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// On collision with the player, applies either a weakness or fragility debuff (randomly chosen).
    /// Ensures each player collider is only affected once per sparkle.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null &&
            other.gameObject.CompareTag("Player") &&
            other.isTrigger && !affecteds.Contains(other))
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.5)
            {
                BuffDebuffManager.Instance.ApplyBuffDebuff(
                    other.gameObject,
                    "Weakness",
                    BuffDebuffManager.BuffDebuffType.WeaknessDebuff,
                    debuffTime,
                    debuffFactor,
                    false
                );
            }
            else
            {
                BuffDebuffManager.Instance.ApplyBuffDebuff(
                    other.gameObject,
                    "Vulnerable",
                    BuffDebuffManager.BuffDebuffType.FragilityDebuff,
                    debuffTime,
                    debuffFactor,
                    false
                );
            }
            affecteds.Add(other);
        }
    }
}
