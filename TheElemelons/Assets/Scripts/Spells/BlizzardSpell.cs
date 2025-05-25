using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlizzardSpell : SpellEffect
{
    float spellLifeTime = 10f;
    float fadeDuration = 2f;
    float freezeImmunitylength = 2f;
    List<SpriteRenderer> spriteRenderers;
    public GameObject Ice;
    Dictionary<GameObject, GameObject> victims;
    Vector2 movement;
    Rigidbody2D rigidbody2D;
    float moveSpeed = 0.5f;
    int dir = 1;
    // Start is called before the first frame update
    void Start()
    {
        movement = transform.position - caster.transform.position;
        rigidbody2D = GetComponent<Rigidbody2D>();
        transform.position = caster.transform.position;
        victims = new Dictionary<GameObject, GameObject>();
        spriteRenderers = new List<SpriteRenderer>();
        for (int i = 0; i < transform.childCount; i++)
        {
            spriteRenderers.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
        }
        StartCoroutine(Expire());
        Debug.Log("start end");
    }
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("some collision");
        Enemy targetScript = other.GetComponent<Enemy>();
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger &&
        targetScript != null &&
        targetScript.GetCurrentState() != EnemyStatus.Staggered &&
        !victims.ContainsKey(other.gameObject))
        {
            Debug.Log("collision");
            FreezeVictim(other.gameObject);
        }
    }
    void FixedUpdate()
    {
        if (Time.frameCount % 19 == 0)
        {
            foreach ((GameObject key, GameObject value) in victims)
            {
                if (victims[key] == null)
                {
                    StartCoroutine(FreezeImmunityCooldown(key));
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
        rigidbody2D.MovePosition(rigidbody2D.position + movement * movement * Time.fixedDeltaTime * dir);
    }
    IEnumerator FreezeImmunityCooldown(GameObject victim)
    {
        yield return new WaitForSeconds(freezeImmunitylength);
        victims.Remove(victim);
    }
    /*
    void OnTriggerExit2D(Collider2D other)
    {
        if (victims.ContainsKey(other.gameObject))
        {
            UnfreezeVictim(other.gameObject);
        }
    }*/
    void FreezeVictim(GameObject victim)
    {
        GameObject IceEffect = Instantiate(Ice, victim.transform.position, Quaternion.Euler(0, 0, 0));
        IceEffect.GetComponent<IceCrystal>().SetVictim(victim);
        victims.Add(victim, IceEffect);
    }
    void UnfreezeVictim(GameObject victim)
    {
        victims[victim].GetComponent<IceCrystal>().Unfreeze();
        victims.Remove(victim);
    }
    /*
    void OnDestroy()
    {
        foreach ((GameObject key, GameObject value) in victims)
        {
            if (victims[key] != null)
            {
                UnfreezeVictim(key);
            }
        }
    }
    */
}
