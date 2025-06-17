using System.Collections;
using UnityEngine;

/// <summary>
/// Floating light spell that orbits the caster, providing illumination and following the player within a certain range.
/// Fades out and is destroyed after its lifetime expires.
/// </summary>
public class LightSpell : SpellEffect
{
    /// <summary>
    /// Duration the light spell remains active before fading out.
    /// </summary>
    public float spellLifeTime = 60f;

    // Duration of the fade-out effect.
    float fadeDuration = 3f;
    // SpriteRenderer for fading the light.
    SpriteRenderer spriteRenderer;
    // Transform of the caster (player).
    Transform casterTransform;
    // Rigidbody2D for movement.
    Rigidbody2D rb;
    // Preferred orbit range from the caster.
    float range = 2f;
    // Maximum allowed distance from the caster before hurrying back.
    float maxDistance = 5f;
    // Tolerance for range transitions.
    float tolerance = 2f;
    // Normal and hurry movement speeds.
    float normalSpeed = 3f;
    float hurrySpeed = 8f;
    float maxSpeed;
    // Directional factors for orbiting.
    int clockWiseFactor = 1;
    float circlefactor = 1;
    // Current range state relative to the caster.
    RangeState currentRangeState;

    /// <summary>
    /// Initializes references, sets orbit direction, and starts the expiration coroutine.
    /// </summary>
    void Start()
    {
        casterTransform = caster.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Expire());
        circlefactor = Random.Range(0, 2) > 0 ? -1 : 1;
        currentRangeState = RangeState.InRange;
    }

    /// <summary>
    /// Handles the spell's lifetime, fades out, and destroys the object.
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        bool dimmed = false;
        transform.Find("Light/HalfShadowMask").gameObject.SetActive(false);
        float fadeTimer = fadeDuration;
        Color c = spriteRenderer.color;
        float chunkiness = 30f;
        while (fadeTimer > 0f)
        {
            if (!dimmed && fadeTimer < fadeDuration * 2 / 3)
            {
                transform.Find("Light/HalfShadowMask").gameObject.SetActive(false);
                dimmed = true;
            }
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Handles orbiting and following logic every physics frame.
    /// </summary>
    void FixedUpdate()
    {
        if (Time.frameCount % 120 == 0 && UnityEngine.Random.Range(0f, 1f) < 0.1f)
        {
            clockWiseFactor *= -1;
        }
        if (Time.frameCount % 90 == 0 && UnityEngine.Random.Range(0f, 1f) < 0.3f)
        {
            circlefactor = UnityEngine.Random.Range(0.8f, 1.2f);
        }
        CalcRangeState();
        ActOnRange();
    }

    // Calculates the current range state relative to the caster.
    void CalcRangeState()
    {
        RangeState nextRangeState = RangeState.Close;
        float dist = (casterTransform.position - transform.position).magnitude;
        maxSpeed = normalSpeed;
        if (dist > maxDistance)
        {
            nextRangeState = RangeState.OutOfRange;
            maxSpeed = hurrySpeed;
        }
        else if (dist > range + tolerance)
        {
            nextRangeState = RangeState.Far;
        }
        else if (dist > range - tolerance)
        {
            nextRangeState = RangeState.InRange;
        }
        if (nextRangeState != currentRangeState)
        {
            if (currentRangeState == RangeState.Far && nextRangeState == RangeState.InRange)
            {
            }
            currentRangeState = nextRangeState;
        }
    }

    // Applies movement logic based on the current range state.
    void ActOnRange()
    {
        Vector2 movement = casterTransform.position - transform.position;
        float dist = movement.magnitude;
        if (currentRangeState == RangeState.OutOfRange)
        {
            rb.AddForce(movement.normalized * Mathf.Abs(range - dist) * 0.5f, ForceMode2D.Impulse);
        }
        else if (currentRangeState == RangeState.Far)
        {
            rb.AddForce(movement.normalized * 0.2f * Mathf.Abs(range - dist), ForceMode2D.Impulse);
        }
        else if (currentRangeState == RangeState.InRange)
        {
            movement = (Vector2)(Quaternion.Euler(0, 0, 90f * clockWiseFactor) * movement) * circlefactor;
            rb.AddForce(movement.normalized * 0.1f * Mathf.Min(1 / (range - dist), tolerance), ForceMode2D.Impulse);
        }
        else if (currentRangeState == RangeState.Close)
        {
            movement = (Vector2)(Quaternion.Euler(0, 0, 90f * clockWiseFactor) * -movement) * circlefactor;
            rb.AddForce(movement.normalized * 0.1f * (range - dist), ForceMode2D.Impulse);
        }
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else if (rb.velocity.magnitude > maxSpeed - tolerance)
        {
            rb.AddForce(-rb.velocity.normalized * 0.1f, ForceMode2D.Impulse);
        }
    }
}