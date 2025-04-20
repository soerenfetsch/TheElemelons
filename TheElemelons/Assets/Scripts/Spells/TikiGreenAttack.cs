using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TikiGreenAttack : SpellEffect
{
    [Header("hit")]
    public float baseDamage = 0.5f;
    public float thrust = 1f;
    public float knockTime = 0.2f;
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
        //parameters
        direction = (Vector2)GameObject.Find("Player/Aim").transform.position - (Vector2)transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        spellType = new ChargeTuple(ChargeTypes.earth, ChargeTypes.notype);
        boxCollider2D.enabled = false;
        //begin charging
        StartCoroutine(ChargeUp());
    }

    void FixedUpdate()
    {
        if (chargedUp)
        {
            transform.position = transform.position + (Vector3)direction * moveSpeed * Time.fixedDeltaTime;
        }
    }
    IEnumerator ChargeUp()
    {
        yield return new WaitForSeconds(chargeUpTime);
        chargedUp = true;
        animator.SetTrigger("Shoot");
        boxCollider2D.enabled = true;
        StartCoroutine(Expire());
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
