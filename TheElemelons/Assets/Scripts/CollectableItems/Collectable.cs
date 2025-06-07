using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for collectable items. Handles general initialization, movement, and player interaction logic.
/// Derived classes implement specific OnCollect behavior.
/// </summary>
public abstract class Collectable : MonoBehaviour
{
    protected float force = 10f;
    protected float range = 1f;
    protected Rigidbody2D player;
    protected Transform playerTransform;
    protected Health playerHealth;
    protected Mana playerMana;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected bool collected;
    protected BoxCollider2D boxCollider2D;
    protected SpriteRenderer spriteRenderer;
    [Header("Hovering Motion")]
    protected float amplitude = 0.1f;
    protected float frequency = 0.3f;
    protected Vector2 fixpoint;
    public GameObject shadowObject;
    protected Transform shadow;
    protected float shadowOffset = 0.7f;
    protected float shadowScale = 1f;

    /// <summary>
    /// Initializes references to player, rigidbody, shadow, animator, and other components.
    /// </summary>
    public void InitializeGeneral()
    {
        player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        fixpoint = rb.position;
        shadow = Instantiate(shadowObject, transform.position - Vector3.down * shadowOffset, Quaternion.Euler(0, 0, 0)).transform;
        shadow.parent = transform;
        shadow.localScale = new Vector3(shadowScale, shadowScale, 1);
        boxCollider2D = GetComponent<BoxCollider2D>();
        playerTransform = player.transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Object is pulled towards player in in range (~2 units), else performs a hovering motion on a fixed location.
    /// If already collected follow the player
    /// </summary>
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
        else
        {
            transform.position = player.position;
        }
    }

    /// <summary>
    /// Calls the abstract method OnCollect() if collision with the player detected.
    /// </summary>
    /// <param name="other">Collider that entered the trigger.</param>
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") &&
            other.isTrigger)
        {
            OnCollect();
        }
    }

    /// <summary>
    /// Abstract method to define collectable-specific behavior when collected by the player.
    /// </summary>
    protected abstract void OnCollect();
}
