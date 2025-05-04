using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    public event Action<Rigidbody2D, Vector2> OnAttackTriggered; // Event for handling attacks
    private bool isEnabled = false; // Spear is only enabled when thrown

    private Vector2 spearDirection;
    private BoxCollider2D spearCollider;
    // Start is called before the first frame update
    void Start()
    {
        spearCollider = GetComponent<BoxCollider2D>();
        spearCollider.enabled = false; // Disable the spear collider at the start
    }

    public void ThrowSpear(Vector2 direction, float speed, float time)
    {
        isEnabled = true; // Enable the spear when thrown
        spearDirection = direction;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Spear: No Rigidbody2D found on the GameObject.");
            return;
        }
        rb.isKinematic = false; // Set the Rigidbody to be dynamic
        rb.gravityScale = 0; // Disable gravity for the Spear
        rb.angularDrag = 0; // Disable angular drag for the Spear
        rb.drag = 0; // Disable drag for the Spear
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze rotation to prevent spinning
        rb.velocity = direction.normalized * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        spearCollider.enabled = true; // Enable the spear collider when thrown
        StartCoroutine(DestroySpearAfterTime(time));
    }

    private IEnumerator DestroySpearAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        spearCollider.enabled = false;
        Destroy(gameObject); // Destroy the spear after the specified time
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isEnabled && other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                OnAttackTriggered?.Invoke(playerRb, spearDirection);
                Destroy(gameObject); // Destroy the spear after hitting the player
            }
        }
    }
}
