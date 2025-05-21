using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// mall Red cloud that disappears after ~0.6s, but then grows big and becomes a big red cloud that obscures sight for
/// the player, as it is rendered above enemies, 
/// player, walls, floor etc. Gameobject is destroyed after spell lifetime runs out ~13s.
/// </summary>
public class WitchRedPoof : MonoBehaviour
{
    Witch witchScript;
    public float spellLifeTime = 13f;
    public float timeUntilBigCloud = 1.2f;
    public float fadeDuration = 5f;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        witchScript = GameObject.Find("Witch").GetComponent<Witch>();
        StartCoroutine(Expire());
        StartCoroutine(MakeBigCloud());
    }
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
    IEnumerator MakeBigCloud()
    {
        yield return new WaitForSeconds(timeUntilBigCloud);
        spriteRenderer.sortingLayerName = "roof";
        spriteRenderer.sortingOrder = 100;
        gameObject.GetComponent<Animator>().SetTrigger("bigCloud");
    }
}
