using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PolygonCollider2D))]
public class RippleEffect : MonoBehaviour
{
    [Header("Ripple Settings")]
    public float speed = 3f;

    private Vector2 direction = Vector2.down;

    public event Action<Rigidbody2D, RippleEffect> OnPlayerHit;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        // Move ripple in direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
        if (other.CompareTag("Player") && hit != null)
        {
            OnPlayerHit?.Invoke(hit, this);
        }
    }

    // Call this from Animation Event at the end of the ripple animation
    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
