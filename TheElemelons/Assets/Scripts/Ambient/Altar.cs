using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an altar that spawns different loot depending on its type and player interaction.
/// - Golden and blood: Spawns a golden collectable heart that increases the player’s max health by 1.
/// - Not golden and blood: Spawns 1-3 collectable hearts that each heal the player’s health by 1.
/// - Golden and pink fluid: Spawns a golden collectable mana star that increases the player’s max mana by 1.
/// - Not golden and pink fluid: Spawns a collectable potion that grants the player a random buff (damage, defense, or speed) for ~2 minutes.
/// </summary>
public class Altar : InteractableObject
{
    [Header("Altar Type Probabilities")]
    /// <summary>
    /// Probability that the altar is golden.
    /// </summary>
    public float goldChance = 0.5f;

    /// <summary>
    /// Probability that the altar contains blood.
    /// </summary>
    public float bloodChance = 0.5f;

    [Header("Bound Components")]
    // Animator for the altar's fluid animation.
    Animator fluidAnimator;
    // Animator for the altar's main animation.
    Animator animator;
    // Collider for the altar's normal state.
    PolygonCollider2D colliderNormal;
    // Collider for the altar's destroyed state.
    PolygonCollider2D colliderDestroyed;
    // The active collider for the altar.
    new PolygonCollider2D collider2D;

    [Header("Flags")]
    /// <summary>
    /// Whether the altar is golden.
    /// </summary>
    public bool gold = false;

    /// <summary>
    /// Whether the altar contains blood.
    /// </summary>
    public bool blood = false;

    // Whether loot has been spawned.
    bool spawned;

    [Header("Loot")]
    /// <summary>
    /// The number of symbols required for max upgrades.
    /// </summary>
    public int symbols = 13;

    // Whether the altar has already been interacted with.
    bool interacted = false;

    /// <summary>
    /// Maximum number of hearts that can be spawned.
    /// </summary>
    public int maxSpawnableHearts = 3;

    /// <summary>
    /// Prefab for a collectable heart (heals player).
    /// </summary>
    public GameObject collectableHeart;

    /// <summary>
    /// Prefab for a collectable max heart (increases max health).
    /// </summary>
    public GameObject collectableMaxHeart;

    /// <summary>
    /// Prefab for a collectable max mana (increases max mana).
    /// </summary>
    public GameObject collectableMaxMana;

    /// <summary>
    /// Prefab for a collectable pink potion (grants random buff).
    /// </summary>
    public GameObject collectablePinkPotion;

    // List of loot prefabs to spawn.
    List<GameObject> lootList;
    // List of rigidbodies for spawned loot.
    List<Rigidbody2D> rbLootList;
    // Reference to the player's health.
    Health playerHealth;
    // Reference to the player's mana.
    Mana playerMana;

    [Header("Movement")]
    // Maximum distance loot can travel.
    float maxDistance = 4f;
    // Lists for loot movement and trajectory.
    List<float> maxDistances;
    List<float> distances;
    List<float> amplitudes;
    List<float> xOffset;
    List<int> xsign;
    List<int> ysign;
    List<Vector2> dirs;
    // Speed at which loot moves.
    float moveSpeed = 3f;

    // Start is called before the first frame update.
    // Initializes altar state, components, and loot lists.
    void Start()
    {
        InitializeRandomProperties();
        InitializeGeneral();
        InitializeLists();
        InitializeCollider();
    }

    /// <summary>
    /// Initializes references to components and player stats, and sets animator flags.
    /// </summary>
    public void InitializeGeneral()
    {
        animator = GetComponent<Animator>();
        fluidAnimator = transform.GetChild(0).GetComponent<Animator>();
        colliderNormal = transform.GetChild(1).GetComponent<PolygonCollider2D>();
        colliderDestroyed = transform.GetChild(2).GetComponent<PolygonCollider2D>();
        GameObject player = GameObject.FindWithTag("Player").gameObject;
        playerHealth = player.GetComponent<Health>();
        playerMana = player.GetComponent<Mana>();
        gold = ExceedsMax() ? false : gold;
        animator.SetBool("Gold", gold);
        fluidAnimator.SetBool("Blood", blood);
    }

    /// <summary>
    /// Checks if the player already exceeds the maximum allowed upgrades.
    /// </summary>
    /// <returns>True if the player exceeds max upgrades, otherwise false.</returns>
    public bool ExceedsMax()
    {
        return gold && ((blood && playerHealth.GetMaxHealth() > symbols) || (!blood && playerMana.GetMaxMana() > symbols));
    }

    // Initializes the collider for the altar.
    void InitializeCollider()
    {
        collider2D = GetComponent<PolygonCollider2D>();
        collider2D.points = colliderNormal.points;
    }

    /// <summary>
    /// Initializes lists used for loot and movement.
    /// </summary>
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

    // Randomly determines if the altar is golden or contains blood based on probabilities.
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
            bool exceeds = ExceedsMax();
            gold = exceeds ? false : gold;
            blood = exceeds ? false : blood;
            StartCoroutine(Destruction());
        }
    }

    // Handles altar destruction animation and triggers loot selection.
    IEnumerator Destruction()
    {
        fluidAnimator.SetTrigger("Bleed");
        yield return new WaitForSeconds(1.1f);
        animator.SetTrigger("Destruction");
        yield return new WaitForSeconds(0.5f);
        collider2D.points = colliderDestroyed.points;
        ChoseLoot();
    }

    /// <summary>
    /// Chooses which loot to spawn based on altar type and player state.
    /// </summary>
    /// <param name="spawn">Whether to spawn the loot immediately.</param>
    /// <returns>List of loot GameObjects to be spawned.</returns>
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

    // Instantiates and initializes loot objects, setting up their movement properties.
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
    /// If the collectables have spawned, calls DoMovement().
    /// </summary>
    void FixedUpdate()
    {
        if (spawned)
        {
            DoMovement();
        }
    }

    /// <summary>
    /// Moves each collectable around in a parabola-shaped trajectory. 
    /// The distance each collectable travels is measured and after a short random distance, 
    /// the movement is interrupted and their collectable scripts become enabled.
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
