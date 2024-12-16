using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public float deathEffectDelay = 1f;

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

