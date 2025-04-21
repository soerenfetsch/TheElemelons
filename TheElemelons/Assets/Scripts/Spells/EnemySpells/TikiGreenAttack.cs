using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// spell that first charges up for short time, then propagates in casting direction
/// deals damage to player if hits player
/// </summary>
public class TikiGreenAttack : SpellEffect
{
    [Header("hit")]
    public float baseDamage = 0.5f;
    public float thrust = 1f;
    public float knockTime = 0.2f;
    [Header("pre-propagation movement")]
    Transform casterTransform;
    Vector2 offset;
    [Header("movement")]
    bool chargedUp = false;
    public float chargeUpTime = 0.3f;
    public float spellLifeTime = 2f;
    public float moveSpeed = 4f;
    Animator animator;
    SpriteRenderer spriteRenderer;
    float fadeDuration = 0.4f;
    Vector2 direction;
    [Header("other")]
    Rigidbody2D rb;
    BoxCollider2D boxCollider2D;

    // Start is called before the first frame update
    void Start()
    {
        //components
        animator = transform.GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody2D>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
        casterTransform = caster.transform;
        //parameters
        Vector2 castpoint = caster.transform.position;
        if (caster.GetComponent<Tiki>() != null)
        {
            castpoint = caster.transform.GetChild(0).transform.position;
            castpoint.y += 1f;
        }
        offset = (Vector2)transform.position - castpoint;
        direction = (Vector2)transform.position - castpoint;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        spellType = new ChargeTuple(ChargeTypes.earth, ChargeTypes.notype);
        boxCollider2D.enabled = false;
        //begin charging
        StartCoroutine(ChargeUp());
    }
    /// <summary>
    /// if not charged up yet, follow caster position
    /// if charged up, propate in casting direction
    /// </summary>
    void FixedUpdate()
    {
        if (chargedUp)
        {
            transform.position = transform.position + (Vector3)direction * moveSpeed * Time.fixedDeltaTime;
        }
        else if (caster != null)
        {
            Vector2 newpos;
            newpos = (Vector2)caster.transform.position + offset;
            if (caster.GetComponent<Tiki>() != null)
            {
                newpos = (Vector2)caster.transform.GetChild(0).transform.position + offset;
                newpos.y += 1f;
            }
            transform.position = newpos;
        }
    }
    /// <summary>
    /// sets charged up bool to true after given time and updates animator
    /// </summary>
    /// <returns></returns>
    IEnumerator ChargeUp()
    {
        yield return new WaitForSeconds(chargeUpTime);
        chargedUp = true;
        animator.SetTrigger("Shoot");
        boxCollider2D.enabled = true;
        StartCoroutine(Expire());
    }
    /// <summary>
    /// waits until spelllifetime expired
    /// after spelllifetime expired, fades sprite gradually until transparent and selfdestructs
    /// </summary>
    /// <returns></returns>
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
    /// if collides with player and already charged up and player isnt already staggered, execute attack and selfdestruct
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (chargedUp &&
            other.gameObject.CompareTag("Player") &&
            other.isTrigger &&
            other.gameObject.GetComponent<Player>().GetCurrentState() != PlayerState.Stagger)
        {
            Vector2 attackDirection = other.transform.position - transform.position;
            CombatManager.ExecuteAttack(caster, other.GetComponent<Rigidbody2D>(), spellType, attackDirection, baseDamage, thrust, knockTime);
            Destroy(gameObject);
        }
        else if (chargedUp &&
            other.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
