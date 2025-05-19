using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Red sparkle effect. Gameobject is destroyed after spelllifetime runs out ~2s. 
/// Deals a weakness or defense Debuff to the player on collision, chosen randomly. Can also do both.
/// </summary>
public class WitchRedSparkles : SpellEffect
{
    public float debuffTime = 10f;
    public float debuffFactor = 1.2f;
    public float spellLifeTime = 2f;
    List<Collider2D> affecteds;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spellType = new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype);
        affecteds = new List<Collider2D>();
        StartCoroutine(Expire());
    }
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null &&
            other.gameObject.CompareTag("Player") &&
            other.isTrigger && !affecteds.Contains(other) &&
            other.gameObject.GetComponent<Player>() != null)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.5)
            {
                BuffDebuffManager.Instance.ApplyBuffDebuff(other.gameObject, "Weakness", BuffDebuffManager.BuffDebuffType.WeaknessDebuff, debuffTime, debuffFactor, false);

            }
            else
            {
                BuffDebuffManager.Instance.ApplyBuffDebuff(other.gameObject, "Vulnerable", BuffDebuffManager.BuffDebuffType.FragilityDebuff, debuffTime, debuffFactor, false);

            }
            affecteds.Add(other);
        }
    }

}
