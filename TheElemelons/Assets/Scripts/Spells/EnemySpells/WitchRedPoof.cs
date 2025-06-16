using System.Collections;
using UnityEngine;

/// <summary>
/// Small red cloud that disappears after ~0.6s, but then grows big and becomes a big red cloud that obscures sight for
/// the player, as it is rendered above enemies, player, walls, floor etc.
/// GameObject is destroyed after spell lifetime runs out (~13s).
/// </summary>
public class WitchRedPoof : MonoBehaviour
{
    // Reference to the Witch script (not used, but cached for potential future use).
    Witch witchScript;

    /// <summary>
    /// Lifetime of the visual effect before destruction.
    /// </summary>
    public float spellLifeTime = 13f;

    /// <summary>
    /// Time after which the cloud grows into a big cloud.
    /// </summary>
    public float timeUntilBigCloud = 1.2f;

    /// <summary>
    /// Duration of the fade-out effect at the end of the lifetime.
    /// </summary>
    public float fadeDuration = 5f;

    // Reference to the SpriteRenderer for fading and sorting.
    SpriteRenderer spriteRenderer;

    /// <summary>
    /// Initializes references, starts expiration and big cloud coroutines.
    /// </summary>
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        witchScript = GameObject.Find("Witch").GetComponent<Witch>();
        StartCoroutine(Expire());
        StartCoroutine(MakeBigCloud());
    }

    /// <summary>
    /// Waits for spellLifeTime, then fades out and destroys the GameObject.
    /// </summary>
    IEnumerator Expire()
    {
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
    /// After timeUntilBigCloud, moves the cloud to the "roof" sorting layer, increases order, and triggers the big cloud animation.
    /// </summary>
    IEnumerator MakeBigCloud()
    {
        yield return new WaitForSeconds(timeUntilBigCloud);
        spriteRenderer.sortingLayerName = "roof";
        spriteRenderer.sortingOrder = 100;
        gameObject.GetComponent<Animator>().SetTrigger("bigCloud");
    }
}
