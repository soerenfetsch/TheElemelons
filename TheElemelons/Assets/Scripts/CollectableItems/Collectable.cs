using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    protected float force = 10f;
    protected float range = 1f;
    protected Rigidbody2D player;
    protected Transform playerTransform;
    protected Health playerHealth;
    protected Rigidbody2D rb;
    protected bool collected;
    protected BoxCollider2D boxCollider2D;
    protected SpriteRenderer spriteRenderer;
    [Header("Hovering Motion")]
    protected float amplitude = 0.1f;
    protected float frequency = 0.3f;
    protected Vector2 fixpoint;
    protected Transform shadow;
    protected float shadowOffset = 0.7f;

    public void InitializeGeneral()
    {
        player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        fixpoint = rb.position;
        shadow = transform.GetChild(0).transform;
        boxCollider2D = GetComponent<BoxCollider2D>();
        playerTransform = player.transform;
    }
    protected void Movement()
    {
        if (!collected)
        {
            Vector3 difference = player.position - rb.position;
            if (Mathf.Abs(difference.magnitude) < range)
            {
                rb.AddForce(difference.normalized * (range - difference.magnitude) * force);
                fixpoint = rb.position;
            }
            else
            {
                Vector2 moving_by = Vector2.up * Mathf.Sin(frequency * Time.frameCount * Time.fixedDeltaTime) * amplitude;
                shadow.position = fixpoint - Vector2.up * shadowOffset;
                rb.MovePosition(fixpoint + moving_by);
            }
        }
    }
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger)
        {
            OnCollect();
        }
    }
    protected abstract void OnCollect();
}
