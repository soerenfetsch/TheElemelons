using System;
using UnityEngine;

/// <summary>
/// Represents a portal that can trigger events and visual effects when the player approaches or interacts.
/// Handles glow animation, activation radius, and portal entry events.
/// </summary>
public class Portal : MonoBehaviour
{
    // Reference to the player's transform.
    Transform playerTransform;

    /// <summary>
    /// Event triggered when the player enters the portal.
    /// </summary>
    public Action OnPortalEnter;

    // Points for glow movement and visual effects.
    Transform pointA;
    Transform pointB;
    Transform glow1;
    Transform glow2;
    GameObject blueLight;
    GameObject lightObject;

    /// <summary>
    /// Speed at which the portal's glow moves.
    /// </summary>
    public float glowMoveSpeed = 5f;

    /// <summary>
    /// Distance within which the portal activates its effects.
    /// </summary>
    public float activationRadius = 4f;

    // Start is called before the first frame update.
    // Initializes references to player, glow, and light objects.
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        blueLight = transform.Find("BlueLightMask/BlueLight").gameObject;
        glow1 = blueLight.transform.Find("Glow1");
        glow2 = blueLight.transform.Find("Glow2");
        pointA = blueLight.transform.Find("PointA");
        pointB = blueLight.transform.Find("PointB");
        lightObject = transform.Find("Light").gameObject;
    }

    // Moves both glow objects along their paths.
    void DoGlowMovement()
    {
        MoveGlow(glow1);
        MoveGlow(glow2);
    }

    // Moves a single glow object upward, looping back to pointA if it passes pointB.
    void MoveGlow(Transform glow)
    {
        Vector2 move_by = Vector2.up * glowMoveSpeed * Time.fixedDeltaTime;
        Vector3 nextPos = glow.position + (Vector3)move_by;
        if (nextPos.y > pointB.position.y)
        {
            nextPos = pointA.position;
        }
        glow.position = nextPos;
    }

    // FixedUpdate is called at a fixed interval.
    // Activates or deactivates portal effects based on player proximity.
    void FixedUpdate()
    {
        if ((playerTransform.position - transform.position).magnitude < activationRadius)
        {
            blueLight.SetActive(true);
            lightObject.SetActive(true);
            DoGlowMovement();
        }
        else
        {
            blueLight.SetActive(false);
            lightObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object.
    /// If the collider belongs to the feet of the player, the portal action is triggered.
    /// </summary>
    /// <param name="other">The Collider2D that has entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Feet"))
        {
            OnPortalEnter?.Invoke();
        }
    }
}
