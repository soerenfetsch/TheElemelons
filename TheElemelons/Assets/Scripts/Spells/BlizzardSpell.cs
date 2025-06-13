using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BlizzardSpell : SpellEffect
{
    float spellLifeTime = 10f;
    float fadeDuration = 2f;
    float freezeImmunitylength = 2f;
    List<SpriteRenderer> spriteRenderers;
    public GameObject Ice;
    List<GameObject> victims;
    Vector2 movement;
    new Rigidbody2D rigidbody2D;
    float moveSpeed = 0.5f;
    int dir = 1;
    float freezeTime = 5f;
    // Start is called before the first frame update
    void Start()
    {
        movement = transform.position - caster.transform.position;
        rigidbody2D = GetComponent<Rigidbody2D>();
        transform.position = caster.transform.position;
        victims = new List<GameObject>();
        spriteRenderers = new List<SpriteRenderer>();
        for (int i = 0; i < 4; i++)
        {
            spriteRenderers.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
        }

        StartCoroutine(Grow(transform.localScale.x));
        Debug.Log("start end");
    }
    /// <summary>
    /// grow cloud until reaches max scale
    /// </summary>
    /// <param name="maxScale"></param>
    /// <returns></returns>
    IEnumerator Grow(float maxScale)
    {
        transform.localScale = new Vector2(0.1f * maxScale, 0.1f * maxScale);
        float duration = 1f;
        float fadeTimer = duration;
        float chunkiness = 2f;
        while (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp(1 - fadeTimer / fadeDuration, 0.1f, 1f);
            transform.localScale = new Vector2(alpha * maxScale, alpha * maxScale);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        GetComponent<CircleCollider2D>().enabled = true;
        StartCoroutine(Expire());
    }
    /// <summary>
    /// self destruct after spelllifetime and fading
    /// </summary>
    /// <returns></returns>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        for (int i = 0; i < spriteRenderers.Count - 1; i++)
        {
            StartCoroutine(fade(spriteRenderers[i]));
        }
        yield return fade(spriteRenderers[spriteRenderers.Count - 1]);
        Destroy(gameObject);
    }
    IEnumerator fade(SpriteRenderer spriteRenderer)
    {
        float fadeTimer = fadeDuration;
        Color c = spriteRenderer.color;
        float chunkiness = 30f;
        while (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
    }
    /// <summary>
    /// freeze enemies
    /// enemies are on a short cooldown before can be frozen again
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("some collision");
        Enemy targetScript = other.GetComponent<Enemy>();
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger &&
        targetScript != null &&
        targetScript.GetCurrentState() != EnemyStatus.Staggered &&
        !victims.Contains(other.gameObject))
        {
            FreezeVictim(other.gameObject);
        }
    }
    /// <summary>
    /// trigger cooldown on unfrozen enemies
    /// cloud movement
    /// </summary>
    void FixedUpdate()
    {
        if (Time.frameCount % 19 == 0)
        {
            foreach (GameObject victim in victims)
            {
                if (victim == null)
                {
                    continue;
                }
                if (BuffDebuffManager.Instance.GetActiveEffects(victim).Where(p => p.name == "BlizzardIce") == null)
                {
                    StartCoroutine(FreezeImmunityCooldown(victim));
                }
            }
        }
        if (Time.frameCount % 60 == 0)
        {
            dir *= -1;
            movement = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 180f)) * movement;
        }
        DoMovement();
    }
    void DoMovement()
    {
        movement = movement.normalized;
        rigidbody2D.MovePosition(rigidbody2D.position + moveSpeed * movement * Time.fixedDeltaTime * dir);
    }
    IEnumerator FreezeImmunityCooldown(GameObject victim)
    {
        yield return new WaitForSeconds(freezeImmunitylength);
        victims.Remove(victim);
    }
    /// <summary>
    /// inflict victim with freeze debuff and add to list
    /// </summary>
    /// <param name="victim"></param>
    void FreezeVictim(GameObject victim)
    {
        BuffDebuffManager.Instance.ApplyBuffDebuff(victim, "BlizzardIce", BuffDebuffManager.BuffDebuffType.FreezeDebuff, freezeTime + fadeDuration * 3f / 4f, 1, false);
        victims.Add(victim);
    }
}
