using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableHeart : MonoBehaviour
{
    float force = 10f;
    float healValue = 1f;
    float range = 1f;
    Rigidbody2D player;
    Transform playerTransform;
    Health playerHealth;
    Rigidbody2D rb;
    public bool collected;
    Animator animator;
    BoxCollider2D boxCollider2D;
    SpriteRenderer spriteRenderer;
    [Header("Hovering Motion")]
    float amplitude = 0.1f;
    float frequency = 0.3f;
    Vector2 fixpoint;
    Transform shadow;
    float shadowOffset = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        playerHealth = player.GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        fixpoint = rb.position;
        shadow = transform.GetChild(0).transform;
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        playerTransform = player.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
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
        else
        {
            transform.position = player.position;
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger)
        {
            playerHealth.AddHealth(healValue);
            StartCoroutine(Expire());
        }
    }
    IEnumerator Expire()
    {
        animator.SetTrigger("heal");
        collected = true;
        boxCollider2D.enabled = false;
        spriteRenderer.sortingOrder = 100;
        Destroy(shadow.gameObject);
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
