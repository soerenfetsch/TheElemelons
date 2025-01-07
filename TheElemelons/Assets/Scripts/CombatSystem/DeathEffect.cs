using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public float deathEffectDelay = 1f;

    /// <summary>
    /// Spawns a death effect at the specified position with a given delay.
    /// This method instantiates the death effect prefab at the specified position and initializes it with the given delay.
    /// </summary>
    /// <param name="prefab">The death effect prefab to instantiate.</param>
    /// <param name="position">The position to spawn the death effect.</param>
    /// <param name="delay">The delay before the death effect is destroyed.</param>
    public static void SpawnDeathEffect(GameObject prefab, Vector3 position, float delay)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        DeathEffect script = instance.GetComponent<DeathEffect>();
        if (script != null)
        {
            script.Initialize(delay); // Delay set before coroutine starts
        }
    }

    public void Initialize(float delay)
    {
        deathEffectDelay = delay;
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(deathEffectDelay);
        Destroy(gameObject);
    }
}

