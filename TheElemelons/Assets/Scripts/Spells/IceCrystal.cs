using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// icecrystal effect for freeze debuff
/// </summary>
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
    // unfreeze after bufflifetime
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
                BuffDebuffManager.Instance.ClearEffect(target, BuffDebuffManager.BuffDebuffType.FreezeDebuff, effectName: "BlizzardIce");
            }
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            dist = distance * alpha;
            mask.transform.position = new Vector3(mask.transform.position.x, maskDestination.transform.position.y + dist, 0);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        Destroy(gameObject);
    }
    /// <summary>
    /// fire spells can melt the ice prematurely
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        SpellEffect spellEffect = other.gameObject.GetComponent<SpellEffect>();
        if (other.CompareTag("Spell") && spellEffect != null && spellEffect.spellType != null && spellEffect.spellType.HasHowMany(ChargeTypes.fire) > 0)
        {
            Unfreeze();
        }
    }
}
