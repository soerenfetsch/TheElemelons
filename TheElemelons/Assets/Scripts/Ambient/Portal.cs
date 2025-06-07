using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a portal that can be activated by the player. Handles visual effects and triggers progression or teleportation when entered.
/// </summary>
public class Portal : MonoBehaviour
{
    Transform playerTransform;
    public Action OnPortalEnter;
    Transform pointA;
    Transform pointB;
    Transform glow1;
    Transform glow2;
    GameObject blueLight;
    GameObject lightObject;
    public float glowMoveSpeed = 5f;
    public float activationRadius = 4f;

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

    void DoGlowMovement()
    {
        MoveGlow(glow1);
        MoveGlow(glow2);
    }

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
