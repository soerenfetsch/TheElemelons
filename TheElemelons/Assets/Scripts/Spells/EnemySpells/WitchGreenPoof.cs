using System.Collections;
using UnityEngine;

/// <summary>
/// Just a visual effect. Small green cloud. Gameobject is destroyed after spelllifetime runs out ~0.6s.
/// </summary>
public class WitchGreenPoof : MonoBehaviour
{
    // Reference to the Witch script (not used, but cached for potential future use).
    Witch witchScript;

    /// <summary>
    /// Lifetime of the visual effect before destruction.
    /// </summary>
    public float spellLifeTime = 0.6f;

    /// <summary>
    /// Initializes the effect by caching the Witch script and starting the expiration coroutine.
    /// </summary>
    void Start()
    {
        witchScript = GameObject.Find("Witch").GetComponent<Witch>();
        StartCoroutine(Expire());
    }

    /// <summary>
    /// Waits for spellLifeTime, then destroys the GameObject.
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
}
