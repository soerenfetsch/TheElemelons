using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpell : SpellEffect
{
    float spellLifeTime = 60f;
    float fadeDuration = 3f;
    SpriteRenderer spriteRenderer;
    Transform casterTransform;
    Rigidbody2D rb;
    float range = 2f;
    float maxDistance = 5f;
    float tolerance = 2f;
    float normalSpeed = 3f;
    float hurrySpeed = 8f;
    float maxSpeed;
    int clockWiseFactor = 1;
    float circlefactor = 1;
    RangeState currentRangeState;
    // Start is called before the first frame update
    void Start()
    {
        casterTransform = caster.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Expire());
        circlefactor = Random.Range(0, 2) > 0 ? -1 : 1;
        try
        {
            Transform darknessLayers = GameObject.Find("DarknessLayers").transform;
            if (darknessLayers != null)
            {
                transform.parent = GameObject.Find("DarknessLayers").transform;
            }
        }
        catch (System.Exception)
        {
            Debug.Log("no darkness layers found");
            throw;
        }
        currentRangeState = RangeState.InRange;
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
    // Update is called once per frame
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
            //rb.AddForce(-rb.velocity.normalized * 0.4f, ForceMode2D.Impulse);
        }
        else if (rb.velocity.magnitude > maxSpeed - tolerance)
        {
            rb.AddForce(-rb.velocity.normalized * 0.1f, ForceMode2D.Impulse);
        }
    }
}
