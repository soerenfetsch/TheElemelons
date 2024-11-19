using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ShardCloud : SpellEffect
{
    [Header("Stats")]
    public int shardCount = 6;
    float shardDelay;
    public float frequency = 0.25f;
    public float spellLifeTime;
    public float spellDamage;
    public float radius = 5f;
    public float thrust = 5f;
    public float damage = 0.25f;
    public float knockTime = 0.5f;
    public ChargeTypes spellType = ChargeTypes.earth;
    [Header("Internal Stuff")]
    Transform playerTransform;
    GameObject[] Shards;
    public GameObject shardEffect;
    Transform castPoint;
    Transform pivot;
    void Start()
    {
        shardDelay = 1/(frequency*((float)shardCount));
        playerTransform = GameObject.Find("Player").transform;
        castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        pivot = GameObject.Find("Player/Aim/Aiming_Pivot").transform;
        Shards = new GameObject[shardCount];
        StartCoroutine(shardCreation());
        StartCoroutine(shardDestruction());

    }
    private IEnumerator shardCreation(){
        Vector3 spawnPos;
        spawnPos = castPoint.position - pivot.position;
        spawnPos.Normalize();
        spawnPos *= radius;
        for (int i = 0; i < shardCount; i++)
        {
            Shards[i] = Instantiate(shardEffect, spawnPos ,Quaternion.Euler(0,0,0));
            Shards[i].GetComponent<Shard>().spellLifeTime = spellLifeTime;
            yield return new WaitForSeconds(shardDelay);
        }
    }
    private IEnumerator shardDestruction() 
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Shards.Count() == 0)
        {
            Destroy(gameObject);
        }
    }
    void FixedUpdate()
    {
        transform.position = playerTransform.position;
    }
}
