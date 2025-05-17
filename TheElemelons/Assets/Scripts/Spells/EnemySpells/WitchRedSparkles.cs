using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchRedSparkles : SpellEffect
{
    public float weaknessTime = 10f;
    public float weaknessFactor = 1.2f;
    public float spellLifeTime = 10f;
    List<Collider2D> affecteds;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype);
        affecteds = new List<Collider2D>();
        StartCoroutine(Expire());
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        float fadeTimer = 2f;
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
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger && !affecteds.Contains(other))
        {
            BuffDebuffManager.Instance.ApplyBuffDebuff(other.gameObject, "Weakness", BuffDebuffManager.BuffDebuffType.WeaknessDebuff, weaknessTime, weaknessFactor, true);
            affecteds.Add(other);
        }
    }

}
