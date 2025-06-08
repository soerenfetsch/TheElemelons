using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Case golden and blood: Spawns a golden collectable heart that increases the player’s max health by 1 on interact.
///Case not golden and  blood: Spawns 1-3 collectable hearts that each heal the player’s health by 1 on interact.
///Case golden and pink fluid: Spawns a golden collectable mana star that increases the player’s max mana by 1 on interact.
///Case not golden and pink fluid: Spawns a collectable potion that grants the player a random buff (damage, defense or speed) 
/// that has a duration of ~2mins on interact.
/// </summary>
public class Altar : InteractableObject
{
    [Header("Altar Type Probabilities")]
    public float goldChance = 0.5f;
    public float bloodChance = 0.5f;

    [Header("Bound Components")]
    Animator fluidAnimator;
    Animator animator;
    PolygonCollider2D colliderNormal;
    PolygonCollider2D colliderDestroyed;
    new PolygonCollider2D collider2D;
    [Header("Flags")]
    public bool gold = false;
    public bool blood = false;
    bool spawned;
    [Header("Loot")]
    bool interacted = false;
    public int maxSpawnableHearts = 3;
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
        InitializeRandomProperties();
        InitializeGeneral();
        InitializeLists();
        InitializeCollider();
    }
    public void InitializeGeneral()
    {
        animator = GetComponent<Animator>();
        fluidAnimator = transform.GetChild(0).GetComponent<Animator>();
        colliderNormal = transform.GetChild(1).GetComponent<PolygonCollider2D>();
        colliderDestroyed = transform.GetChild(2).GetComponent<PolygonCollider2D>();
        animator.SetBool("Gold", gold);
        fluidAnimator.SetBool("Blood", blood);
    }
    void InitializeCollider()
    {
        collider2D = GetComponent<PolygonCollider2D>();
        collider2D.points = colliderNormal.points;
    }
    public void InitializeLists()
    {
        lootList = new List<GameObject>();
        rbLootList = new List<Rigidbody2D>();
        distances = new List<float>();
        amplitudes = new List<float>();
        maxDistances = new List<float>();
        xOffset = new List<float>();
        dirs = new List<Vector2>();
        xsign = new List<int>();
        ysign = new List<int>();
    }

    private void InitializeRandomProperties()
    {
        if (Random.Range(0f, 1f) < goldChance)
        {
            gold = true;
        }
        if (Random.Range(0f, 1f) < bloodChance)
        {
            blood = true;
        }
    }

    /// <summary>
    /// Plays self-destruct animation and spawns loot depending on altar type. 
    /// The “Collectable”-script component of the collectables is deactivated first.
    /// </summary>
    public override void Interact()
    {
        if (!interacted)
        {
            interacted = true;
            StartCoroutine(Destruction());
        }
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
    public List<GameObject> ChoseLoot(bool spawn = true)
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
            int count = UnityEngine.Random.Range(1, maxSpawnableHearts + 1);
            for (int i = 0; i < count; i++)
            {
                lootList.Add(collectableHeart);
            }
        }
        else
        {
            lootList.Add(collectablePinkPotion);
        }
        if (spawn)
        {
            SpawnLoot();
        }
        return lootList;
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
    /// <summary>
    ///  if the collectables have spawned, calls DoMovement().
    /// </summary>
    void FixedUpdate()
    {
        if (spawned)
        {
            DoMovement();
        }
    }
    /// <summary>
    /// Moves each collectable around in a parabola shaped trajectory. 
    /// The distance each collectable travels is measured and after a short random distance, 
    /// the movement is interupted and their collectable scripts become enabled.
    /// </summary>
    void DoMovement()
    {
        for (int i = 0; i < rbLootList.Count; i++)
        {
            if (distances[i] > 0f)
            {
                Vector2 dir = new Vector2(xsign[i], ysign[i] * 2 * amplitudes[i] * (maxDistances[i] - distances[i] - xOffset[i]));
                Vector2 moveby = dir.normalized * moveSpeed * Time.fixedDeltaTime;
                rbLootList[i].MovePosition(rbLootList[i].position + moveby);
                distances[i] -= moveby.magnitude;
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
    /// <summary>
    /// After all spawned collectables have travelled their desired distance and their collectable 
    /// scripts have been activated, Update() deactivates the altar’s altar script.
    /// </summary>
    void Update()
    {
        if (spawned && rbLootList.Count == 0)
        {
            transform.GetComponent<Altar>().enabled = false;
        }
    }

}
