using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : InteractableObject
{
    [Header("Bound Components")]
    Animator fluidAnimator;
    Animator animator;
    PolygonCollider2D colliderNormal;
    PolygonCollider2D colliderDestroyed;
    PolygonCollider2D collider2D;
    [Header("Flags")]
    public bool gold = false;
    public bool blood = false;
    bool spawned;
    [Header("Loot")]
    public GameObject collectableHeart;
    public GameObject collectableMaxHeart;
    public GameObject collectableMaxMana;
    public GameObject collectablePinkPotion;
    List<GameObject> lootList;
    List<Rigidbody2D> rbLootList;
    [Header("Movement")]
    float maxDistance = 4f;
    List<float> maxDistances;
    List<float> distances;
    List<float> amplitudes;
    List<float> xOffset;
    List<int> xsign;
    List<int> ysign;
    List<Vector2> dirs;
    float moveSpeed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        InitializeGeneral();
    }
    void InitializeGeneral()
    {
        animator = GetComponent<Animator>();
        fluidAnimator = transform.GetChild(0).GetComponent<Animator>();
        colliderNormal = transform.GetChild(1).GetComponent<PolygonCollider2D>();
        colliderDestroyed = transform.GetChild(2).GetComponent<PolygonCollider2D>();
        animator.SetBool("Gold", gold);
        fluidAnimator.SetBool("Blood", blood);
        lootList = new List<GameObject>();
        rbLootList = new List<Rigidbody2D>();
        distances = new List<float>();
        amplitudes = new List<float>();
        maxDistances = new List<float>();
        xOffset = new List<float>();
        dirs = new List<Vector2>();
        collider2D = GetComponent<PolygonCollider2D>();
        collider2D.points = colliderNormal.points;
        xsign = new List<int>();
        ysign = new List<int>();
    }
    public override void Interact()
    {
        Debug.Log("interact altar");
        StartCoroutine(Destruction());
    }
    IEnumerator Destruction()
    {
        fluidAnimator.SetTrigger("Bleed");
        yield return new WaitForSeconds(1.1f);
        animator.SetTrigger("Destruction");
        yield return new WaitForSeconds(0.5f);
        collider2D.points = colliderDestroyed.points;
        ChoseLoot();
    }
    void ChoseLoot()
    {
        // add small debuff chance
        if (gold && blood)
        {
            lootList.Add(collectableMaxHeart);
        }
        else if (gold && !blood)
        {
            lootList.Add(collectableMaxMana);
        }
        else if (blood)
        {
            int count = UnityEngine.Random.Range(1, 4);
            for (int i = 0; i < count; i++)
            {
                lootList.Add(collectableHeart);
            }
        }
        else
        {
            lootList.Add(collectablePinkPotion);
        }
        SpawnLoot();
    }
    void SpawnLoot()
    {

        for (int i = 0; i < lootList.Count; i++)
        {
            GameObject current = Instantiate(lootList[i], transform.position, Quaternion.Euler(0, 0, 0));
            current.GetComponent<Collectable>().enabled = false;
            rbLootList.Add(current.GetComponent<Rigidbody2D>());
            amplitudes.Add(UnityEngine.Random.Range(maxDistance / 16f, maxDistance / 8f));
            float dist = UnityEngine.Random.Range(maxDistance / 3, maxDistance);
            xOffset.Add(UnityEngine.Random.Range(maxDistance / 16f, maxDistance / 8f));
            xsign.Add((UnityEngine.Random.Range(0, 2) < 1 ? -1 : 1));
            ysign.Add((UnityEngine.Random.Range(0, 2) < 1 ? -1 : 1));
            distances.Add(dist);
            maxDistances.Add(dist);
            dirs.Add(new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized);
        }
        spawned = true;
    }
    void FixedUpdate()
    {
        if (spawned)
        {
            DoMovement();
        }
    }
    void DoMovement()
    {
        for (int i = 0; i < rbLootList.Count; i++)
        {
            if (distances[i] > 0f)
            {
                Vector2 dir = new Vector2(xsign[i], ysign[i] * 2 * amplitudes[i] * (maxDistances[i] - distances[i] - xOffset[i]));
                Debug.Log(dir);
                Vector2 moveby = dir.normalized * moveSpeed * Time.fixedDeltaTime;
                rbLootList[i].MovePosition(rbLootList[i].position + moveby);
                distances[i] -= moveby.magnitude;
                Debug.Log(distances[i]);
            }
            else
            {
                rbLootList[i].GetComponent<Collectable>().enabled = true;
                rbLootList.Remove(rbLootList[i]);
                distances.Remove(distances[i]);
                dirs.Remove(dirs[i]);
                amplitudes.Remove(amplitudes[i]);
                maxDistances.Remove(maxDistances[i]);
                xOffset.Remove(xOffset[i]);
                xsign.Remove(xsign[i]);
                ysign.Remove(ysign[i]);
            }

        }
    }
    void GrantBuffs()
    {
        //TODO: BuffEffect
    }
    void Update()
    {
        if (spawned && rbLootList.Count == 0)
        {
            transform.GetComponent<Altar>().enabled = false;
        }
    }

}
