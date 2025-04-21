using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikiHealCloud : SpellEffect
{
    public float spellLifeTime = 3f;
    public float totalHeal = 2f;
    Transform target;
    Health targetHealth;
    // Start is called before the first frame update
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
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
    }

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
