using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleSpell : SpellEffect
{
    public float spellLifeTime = 10f;
    public float puddleSpawningTime = 0.5f;
    public int maxPuddleCount = 6;
    Transform playerTransform;
    public GameObject puddle;
    // Start is called before the first frame update
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.water, ChargeTypes.earth);
        playerTransform = GameObject.Find("Player").transform;
        StartCoroutine(Expire());
        StartCoroutine(SpawnPuddles());
    }
    IEnumerator SpawnPuddles()
    {
        for (int puddleCount = 0; puddleCount < maxPuddleCount; puddleCount++)
        {
            CombatManager.InstantiateSpell(puddle, gameObject, playerTransform.position, Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(puddleSpawningTime);
        }
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
