using System.Collections;
using UnityEngine;

/// <summary>
/// spell that heals 1 nearest enemy that isnt tiki
/// healing occurs gradually over time
/// </summary>
public class TikiHealCloud : SpellEffect
{
    public float spellLifeTime = 3f;
    public float totalHeal = 2f;
    Transform target;
    Health targetHealth;
    /// <summary>
    /// at start, select closest enemy that isnt tiki as target
    /// </summary>
    void Start()
    {
        StartCoroutine(SelfDestruct());
        // Select closest enemy
        LayerMask friendsMask = LayerMask.GetMask("Enemies");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 6f, friendsMask);
        float distance = Mathf.Infinity;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.GetComponent<Health>() != null &&
            collider.gameObject.GetComponent<TikiBody>() == null &&
            collider.gameObject.GetComponent<Tiki>() == null &&
            collider.gameObject.GetComponent<TikiHead>() == null)
            {
                Transform currentTransform = collider.transform;
                if (Mathf.Abs((currentTransform.position - transform.position).magnitude) < distance)
                {
                    target = currentTransform;
                    distance = Mathf.Abs((currentTransform.position - transform.position).magnitude);
                }
            }
        }
        if (target == null)
        {
            Destroy(gameObject);
        }
        targetHealth = target.GetComponent<Health>();
        StartCoroutine(DoHeal());
    }
    /// <summary>
    /// selfdestruct after spelllifetime runs out
    /// </summary>
    /// <returns></returns>
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
    /// <summary>
    /// follow target postion each update
    /// </summary>
    void Update()
    {
        transform.position = target.position;
    }
    /// <summary>
    /// heals target gradually over time by set amount of health
    /// </summary>
    /// <returns></returns>
    IEnumerator DoHeal()
    {
        float elapsed = 0f;
        while (elapsed < spellLifeTime)
        {
            elapsed += Time.deltaTime;
            targetHealth.AddHealth(totalHeal / spellLifeTime * Time.deltaTime);
            yield return Time.deltaTime;
        }
    }
}
