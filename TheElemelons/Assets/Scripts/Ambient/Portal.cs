using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Action OnPortalEnter;
    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object.
    /// If the collider belongs to the feet of the player, the player the portal action is triggered.
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
