using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Cloud that obstructs the enemy's sight.
/// Enemy will be inflicted with confusion if in the cloud.
/// Player receives hidden buff if in the cloud.
/// </summary>
public class WaterVaporSpell : SpellEffect
{
    // Lifetime of the spell before it fades out and is destroyed.
    float spellLifeTime = 10f;
    // Duration of the fade-out effect.
    float fadeDuration = 2f;
    // SpriteRenderer for fading the cloud.
    SpriteRenderer spriteRenderer;
    /// <summary>
    /// Prefab for the question mark effect (confusion).
    /// </summary>
    public GameObject questionMark;
    // Tracks which victims have been affected (for possible future use).
    Dictionary<GameObject, GameObject> victims;
    // Collider for applying buffs/debuffs.
    CircleCollider2D circleCollider2D;
    // Reference to the player.
    Player player;

    /// <summary>
    /// Initializes the cloud's position, disables collider, and starts expiration coroutine.
    /// </summary>
    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        transform.position = caster.transform.position;
        player = GameObject.Find("Player").GetComponent<Player>();
        victims = new Dictionary<GameObject, GameObject>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider2D.enabled = false;
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Handles the spell's lifetime, enables collider, fades out, and destroys the object.
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(2.6f);
        StartCoroutine(ColliderOnOff());
        circleCollider2D.enabled = true;
        yield return new WaitForSeconds(spellLifeTime);
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
        Destroy(gameObject);
    }

    /// <summary>
    /// Switches collider on and off to reapply buffs/debuffs.
    /// </summary>
    IEnumerator ColliderOnOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            circleCollider2D.enabled = !circleCollider2D.enabled;
        }
    }

    /// <summary>
    /// Applies a short confusion debuff to enemies or a hidden buff to the player when entering the cloud.
    /// </summary>
    /// <param name="other">The collider entering the cloud.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy targetScript = other.GetComponent<Enemy>();
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger &&
            targetScript != null && other.GetComponent<Health>() != null &&
            targetScript.GetCurrentState() != EnemyStatus.Staggered)
        {
            //ConfuseVictim(other.gameObject); if !victims.ContainsKey(other.gameObject)
            Debug.Log("Confusing : " + other.gameObject.name);
            BuffDebuffManager.Instance.ApplyBuffDebuff(other.gameObject, "WaterVaporConfuse", BuffDebuffManager.BuffDebuffType.ConfusedDebuff, 0.8f, 1, false);
        }
        if (other.gameObject.CompareTag("Player") && other.GetComponent<Player>() != null)
        {
            BuffDebuffManager.Instance.ApplyBuffDebuff(other.gameObject, "WaterVaporHide", BuffDebuffManager.BuffDebuffType.HiddenBuff, 0.8f, 1, false);
        }
    }

    /// <summary>
    /// Turns off the cloud's light effect after a short delay.
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
