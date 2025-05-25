using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCrystal : MonoBehaviour
{
    bool unfreezing = false;
    float freezeTime = 5f;
    float spellLifeTime;
    Transform mask;
    Transform maskDestination;
    Transform ice;
    public GameObject target;
    Enemy targetScript;
    float fadeDuration = 2f;
    // Start is called before the first frame update
    void Start()
    {
        ice = transform.GetChild(0);
        mask = transform.GetChild(1);
        maskDestination = transform.GetChild(2);
        StartCoroutine(Expire());
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(freezeTime);
        Unfreeze();
    }
    public void SetVictim(GameObject victim)
    {
        target = victim;
        transform.localScale = victim.transform.localScale;
        transform.position = target.transform.position + (Vector3)Vector2.down * 0.1f;
        transform.parent = target.transform;
        targetScript = victim.GetComponent<Enemy>();
        targetScript.DoFreezeUnFreeze(true);
        target.transform.position = transform.position;
    }
    public void Unfreeze()
    {
        if (!unfreezing)
        {
            StartCoroutine(UnfreezeCoroutine());
            unfreezing = true;
        }

    }
    IEnumerator UnfreezeCoroutine()
    {
        float fadeTimer = fadeDuration;
        Vector3 c = mask.transform.position;
        float distance = (mask.transform.position - maskDestination.transform.position).magnitude;
        float dist = distance;
        float chunkiness = 30f;
        while (fadeTimer > 0f)
        {
            if (fadeTimer < fadeDuration / 4f)
            {
                targetScript.DoFreezeUnFreeze(false);
            }
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            dist = distance * alpha;
            mask.transform.position = new Vector3(mask.transform.position.x, maskDestination.transform.position.y + dist, 0);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Spell") && other.gameObject.GetComponent<SpellEffect>().spellType.HasHowMany(ChargeTypes.fire) > 0)
        {
            Unfreeze();
        }
    }
}
