using System.Collections;
using UnityEngine;

/// <summary>
/// Visual effect for the freeze debuff ("BlizzardIce").
/// Handles the ice crystal animation, unfreezing logic, and interaction with fire spells.
/// </summary>
public class IceCrystal : MonoBehaviour
{
    // Whether the unfreezing process has started.
    bool unfreezing = false;

    // Duration the freeze effect lasts before auto-unfreeze.
    float freezeTime = 5f;

    // (Unused) Lifetime of the spell.
    float spellLifeTime;

    // Reference to the mask transform (for unfreeze animation).
    Transform mask;

    // Reference to the mask destination transform (for unfreeze animation).
    Transform maskDestination;

    // Reference to the ice visual transform.
    Transform ice;

    /// <summary>
    /// The frozen target GameObject.
    /// </summary>
    public GameObject target;

    // Reference to the frozen target's Enemy script.
    Enemy targetScript;

    // Duration of the fade-out animation when unfreezing.
    float fadeDuration = 2f;

    /// <summary>
    /// Initializes child references and starts the freeze timer.
    /// </summary>
    void Start()
    {
        ice = transform.GetChild(0);
        mask = transform.GetChild(1);
        maskDestination = transform.GetChild(2);
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Waits for freezeTime, then triggers unfreeze.
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(freezeTime);
        Unfreeze();
    }

    /// <summary>
    /// Sets the frozen victim, attaches the effect, and aligns transforms.
    /// </summary>
    /// <param name="victim">The GameObject to freeze.</param>
    public void SetVictim(GameObject victim)
    {
        target = victim;
        transform.localScale = victim.transform.localScale;
        transform.position = target.transform.position + (Vector3)Vector2.down * 0.1f;
        transform.parent = target.transform;
        targetScript = victim.GetComponent<Enemy>();
        target.transform.position = transform.position;
    }

    /// <summary>
    /// Initiates the unfreezing process if not already started.
    /// </summary>
    public void Unfreeze()
    {
        if (!unfreezing)
        {
            StartCoroutine(UnfreezeCoroutine());
            unfreezing = true;
        }
    }

    /// <summary>
    /// Animates the mask moving, fades out, and clears the freeze debuff.
    /// </summary>
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
                // Remove the freeze debuff from the target.
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
    /// Allows fire spells to melt the ice prematurely.
    /// </summary>
    /// <param name="other">Collider that enters the ice crystal's trigger.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        SpellEffect spellEffect = other.gameObject.GetComponent<SpellEffect>();
        if (other.CompareTag("Spell") && spellEffect != null && spellEffect.spellType != null && spellEffect.spellType.HasHowMany(ChargeTypes.fire) > 0)
        {
            Unfreeze();
        }
    }
}
