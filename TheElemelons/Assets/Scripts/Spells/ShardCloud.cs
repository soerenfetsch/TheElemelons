using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Shard class inherits from Spell Effect
/// Shard cloud spawns and manages set number of Shard Spell Effects
/// </summary>
public class ShardCloud : SpellEffect
{
    [Header("Stats")]
    public int shardCount = 6;
    float shardDelay;
    public float frequency = 0.25f;
    public float spellLifeTime;
    public float radius = 5f;
    [Header("Hit")]
    public float thrust = 5f;
    public float damage = 0.25f;
    public float knockTime = 0.5f;
    [Header("Internal Stuff")]
    Transform playerTransform;
    GameObject[] Shards;
    public GameObject shardEffect;
    Transform castPoint;
    Transform pivot;
    /// <summary>
    /// initialize spelltype (wind earth), start coroutine to spawn shards and self destruct timer
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.wind,ChargeTypes.earth);
        shardDelay = 1/(frequency*((float)shardCount));
        playerTransform = GameObject.Find("Player").transform;
        castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        pivot = GameObject.Find("Player/Aim/Aiming_Pivot").transform;
        Shards = new GameObject[shardCount];
        StartCoroutine(shardCreation());
        StartCoroutine(shardDestruction());

    }
    /// <summary>
    /// create set number of shards
    /// wait between each shard instantiating to that they all are equally spaced out due to their rotation around player
    /// </summary>
    private IEnumerator shardCreation(){
        yield return new WaitForSeconds(0.1f);
        Vector3 spawnPos;
        spawnPos = castPoint.position - pivot.position;
        spawnPos.Normalize();
        spawnPos *= radius;
        for (int i = 0; i < shardCount; i++)
        {
            Shards[i] = Instantiate(shardEffect, spawnPos ,Quaternion.Euler(0,0,0));
            Shards[i].GetComponent<Shard>().spellType = spellType;
            Shards[i].GetComponent<Shard>().shardCloud = gameObject.GetComponent<ShardCloud>();
            yield return new WaitForSeconds(shardDelay);
        }
    }
    /// <summary>
    /// destroy shard cloud after spelllifetime runs out
    /// </summary>
    private IEnumerator shardDestruction() 
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }

    /// <summary>
    /// selfdestruct if no shards are left
    /// </summary>
    void Update()
    {
        if (Shards.Count() == 0)
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// adapt transform position to player position
    /// </summary>
    void FixedUpdate()
    {
        transform.position = playerTransform.position;
    }
}
