using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the visual and timing logic for death effects (e.g., particle effects, animations) when an entity dies.
/// Can be spawned at a given position and destroyed after a specified delay.
/// </summary>
public class DeathEffect : MonoBehaviour
{
    /// <summary>
    /// The delay (in seconds) before the death effect is destroyed.
    /// </summary>
    public float deathEffectDelay = 1f;

    /// <summary>
    /// Spawns a death effect at the specified position with a given delay.
    /// This method instantiates the death effect prefab at the specified position and initializes it with the given delay.
    /// </summary>
    /// <param name="prefab">The death effect prefab to instantiate.</param>
    /// <param name="position">The position to spawn the death effect.</param>
    /// <param name="delay">The delay before the death effect is destroyed. Must be >= 0.</param>
    public static void SpawnDeathEffect(GameObject prefab, Vector3 position, float delay)
    {
        // Assert that delay is >= 0
        if (delay < 0)
        {
            Debug.LogError("Delay must be greater than or equal to 0.");
            return;
        }
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        DeathEffect script = instance.GetComponent<DeathEffect>();
        if (script != null)
        {
            script.Initialize(delay); // Delay set before coroutine starts
        }
    }

    /// <summary>
    /// Initializes the death effect with a specified delay.
    /// </summary>
    /// <param name="delay">The delay before the death effect is destroyed.</param>
    public void Initialize(float delay)
    {
        deathEffectDelay = delay;
        StartCoroutine(SelfDestruct());
    }

    /// <summary>
    /// Coroutine that destroys the death effect after the specified delay.
    /// </summary>
    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(deathEffectDelay);
        Destroy(gameObject);
    }
}

