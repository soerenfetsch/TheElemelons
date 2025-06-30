using System.Collections;
using UnityEngine;
/// <summary>
/// PuddleSpell class inherits from Spell Effect
/// spawns given number of buddles
/// </summary>
public class PuddleSpell : SpellEffect
{
    public float spellLifeTime = 10f;
    public float puddleSpawningTime = 0.5f;
    public int maxPuddleCount = 6;
    Transform playerTransform;
    public GameObject puddle;
    /// <summary>
    /// initialize spelltype (water earth), start puddle spawning and expiration coroutine
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.water, ChargeTypes.earth);
        playerTransform = GameObject.Find("Player").transform;
        StartCoroutine(Expire());
        StartCoroutine(SpawnPuddles());
    }
    /// <summary>
    /// spawn given number of puddles with given time interval
    /// </summary>
    IEnumerator SpawnPuddles()
    {
        for (int puddleCount = 0; puddleCount < maxPuddleCount; puddleCount++)
        {
            CombatManager.InstantiateSpell(puddle, gameObject, playerTransform.position, Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(puddleSpawningTime);
        }
    }
    /// <summary>
    /// Expire after spellLifeTime
    /// </summary>
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
}
