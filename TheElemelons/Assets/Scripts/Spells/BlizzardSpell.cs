using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a blizzard spell that creates a freezing cloud.
/// Freezes enemies on contact, applies freeze debuff, and self-destructs after a duration.
/// </summary>
public class BlizzardSpell : SpellEffect
{
    // Lifetime of the spell before it self-destructs.
    float spellLifeTime = 10f;
    // Duration of the fade-out effect when expiring.
    float fadeDuration = 2f;
    // Duration of freeze immunity for enemies after being frozen.
    float freezeImmunitylength = 2f;
    // List of sprite renderers for fading out the cloud.
    List<SpriteRenderer> spriteRenderers;
    /// <summary>
    /// Prefab for the ice effect applied to frozen enemies.
    /// </summary>
    public GameObject Ice;
    // List of currently frozen victims.
    List<GameObject> victims;
    // Current movement vector of the cloud.
    Vector2 movement;
    // Rigidbody2D for moving the cloud.
    new Rigidbody2D rigidbody2D;
    // Movement speed of the cloud.
    float moveSpeed = 0.5f;
    // Direction multiplier for cloud movement.
    int dir = 1;
    // Duration of the freeze debuff applied to enemies.
    float freezeTime = 5f;

    /// <summary>
    /// Initializes movement, sprite renderers, and starts the cloud growth coroutine.
    /// </summary>
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
    /// Grows the cloud until it reaches its maximum scale, then enables the collider and starts expiration.
    /// </summary>
    /// <param name="maxScale">Maximum scale to reach.</param>
    /// <returns>Coroutine enumerator.</returns>
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
    /// Self-destructs the spell after its lifetime and fades out the cloud.
    /// </summary>
    /// <returns>Coroutine enumerator.</returns>
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

    // Fades out a single sprite renderer over fadeDuration.
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
    /// Freezes enemies on contact if not already frozen or staggered.
    /// </summary>
    /// <param name="other">Collider that entered the cloud.</param>
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
    /// Handles cloud movement and freeze immunity cooldown checks.
    /// </summary>
    void FixedUpdate()
    {
        // Periodically check for freeze immunity expiration.
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
        // Randomly change movement direction.
        if (Time.frameCount % 60 == 0)
        {
            dir *= -1;
            movement = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 180f)) * movement;
        }
        DoMovement();
    }

    // Moves the cloud in the current direction.
    void DoMovement()
    {
        movement = movement.normalized;
        rigidbody2D.MovePosition(rigidbody2D.position + moveSpeed * movement * Time.fixedDeltaTime * dir);
    }

    // Removes freeze immunity from a victim after a cooldown.
    IEnumerator FreezeImmunityCooldown(GameObject victim)
    {
        yield return new WaitForSeconds(freezeImmunitylength);
        victims.Remove(victim);
    }

    /// <summary>
    /// Inflicts the freeze debuff on a victim and adds them to the frozen list.
    /// </summary>
    /// <param name="victim">The enemy GameObject to freeze.</param>
    void FreezeVictim(GameObject victim)
    {
        BuffDebuffManager.Instance.ApplyBuffDebuff(
            victim,
            "BlizzardIce",
            BuffDebuffManager.BuffDebuffType.FreezeDebuff,
            freezeTime + fadeDuration * 3f / 4f,
            1,
            false
        );
        victims.Add(victim);
    }
}