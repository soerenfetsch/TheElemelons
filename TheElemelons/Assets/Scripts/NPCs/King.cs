using System.Collections.Generic;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Represents the King NPC, which can walk in various patterns, interact with the player, and trigger dialogues.
/// Implements both walking and idle behaviors, and manages dialogue progression based on game state.
=======
/// Enum for the King's walking pattern.
>>>>>>> Stashed changes
/// </summary>
public enum WalkPattern
{
    /// <summary>Walks in a circle.</summary>
    Circle,
    /// <summary>Walks in a horizontal line.</summary>
    HLine,
    /// <summary>Walks in a vertical line.</summary>
    VLine,
    /// <summary>Walks randomly within bounds.</summary>
    Random,
    /// <summary>Walks to a single destination.</summary>
    Destination,
    /// <summary>Walks to multiple destinations in sequence.</summary>
    MultipleDestinations
}

<<<<<<< Updated upstream
public class King : DialogueTrigger
{
    [Header("Dialogues")]
=======
/// <summary>
/// King NPC: handles dialogue, progression, and autonomous movement with various walk patterns.
/// </summary>
public class King : DialogueTrigger
{
    [Header("Dialogues")]
    /// <summary>
    /// List of introductory dialogues (not looped).
    /// </summary>
>>>>>>> Stashed changes
    public List<DialogueData> intro;
    /// <summary>
    /// Reference to the tree enemy GameObject (used for progression).
    /// </summary>
    public GameObject treeEnemy;
    /// <summary>
    /// List of thank-you dialogues.
    /// </summary>
    public List<DialogueData> thankYou;
    /// <summary>
    /// List of standard (looped) dialogues.
    /// </summary>
    public List<DialogueData> standardDialogue;
    /// <summary>
    /// List of end-of-castle dialogues.
    /// </summary>
    public List<DialogueData> endDialogue;

    // Tracks which dialogue lists should loop (true) or not (false).
    Dictionary<List<DialogueData>, bool> dialogueDictionary;
    // Tracks if the thank-you logic has already been triggered.
    bool alreadyDone = false;
    bool alreadyThanked = false;

    [Header("Walk Patterns")]
    /// <summary>
    /// Distance to walk before changing direction or pattern.
    /// </summary>
    public float walkDist = 3f;
    // Progress along the current walk segment.
    float currentWalkProgress = 0f;
    /// <summary>
    /// Walking speed of the King.
    /// </summary>
    public float walkSpeed = 1.5f;
    /// <summary>
    /// Current walk pattern.
    /// </summary>
    public WalkPattern currentWalkPattern = WalkPattern.Circle;
    /// <summary>
    /// Destination for Destination walk pattern.
    /// </summary>
    public Vector2 destination = Vector2.zero;
    /// <summary>
    /// List of destinations for MultipleDestinations pattern.
    /// </summary>
    public List<Vector2> destinations;
    // Index of the current destination in MultipleDestinations pattern.
    int destinationsIndex = 0;
    /// <summary>
    /// Maximum allowed position for random walking.
    /// </summary>
    public Vector2 walkMaxLimit;
    /// <summary>
    /// Minimum allowed position for random walking.
    /// </summary>
    public Vector2 walkMinLimit;
    /// <summary>
    /// Home position to return to if out of bounds.
    /// </summary>
    public Vector2 homePosition;

    [Header("Movement")]
    // Rigidbody2D for movement.
    private Rigidbody2D rb;
    // Animator for controlling animation states.
    private Animator animator;
    /// <summary>
    /// Current movement vector.
    /// </summary>
    public Vector2 movement;

    /// <summary>
<<<<<<< Updated upstream
    /// Unity Start method. Initializes components, sets initial state, and prepares dialogue dictionary.
=======
    /// Unity Start method. Initializes components and dialogue loop dictionary.
>>>>>>> Stashed changes
    /// </summary>
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        ChangeState(NPCStatus.Walking);
        dialogueDictionary = new Dictionary<List<DialogueData>, bool>();
        dialogueDictionary.Add(intro, false);
        dialogueDictionary.Add(thankYou, false);
        dialogueDictionary.Add(standardDialogue, true);
        dialogueDictionary.Add(endDialogue, true);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Unity Update method. Updates the King's dialogue list based on the current game progression stage.
=======
    /// Unity Update method. Handles dialogue switching and progression logic based on game stage.
>>>>>>> Stashed changes
    /// </summary>
    void Update()
    {
        switch (GameManager.Instance.CurrentProgressionStage)
        {
            case GameProgressionStage.CASTLE_INTRODUCTION:
                if (treeEnemy != null)
                {
                    basic_dialogues_list = intro;
                }
                else
                {
                    basic_dialogues_list = thankYou;
                    if (!alreadyDone)
                    {
                        alreadyDone = true;
                        GameObject.Find("MustTalkToKing").GetComponent<DialogOnWalkInTrigger>().otherCondition = true;
                        GameObject.Find("WayTooDark").GetComponent<DialogOnWalkInTrigger>().otherCondition = false;
                    }
                }
                break;
            case GameProgressionStage.CASTLE_END:
                basic_dialogues_list = endDialogue;
                break;
            case GameProgressionStage.CASTLE_LATE:
                basic_dialogues_list = standardDialogue;
                break;
            case GameProgressionStage.EARTH_LATE_BOSS:
                basic_dialogues_list = standardDialogue;
                break;
            case GameProgressionStage.EARTH_LATE:
                basic_dialogues_list = standardDialogue;
                break;
            default:
                basic_dialogues_list = standardDialogue;
                break;
        }
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles player interaction with the King. Triggers dialogue and progression changes as needed.
=======
    /// Handles player interaction with the King, including progression logic.
>>>>>>> Stashed changes
    /// </summary>
    public override void Interact()
    {
        base.Interact();
        if (basic_dialogues_list == thankYou && !alreadyThanked)
        {
            GameManager.Instance.ChangeProgressionStage(GameProgressionStage.CASTLE_EARLY);
            alreadyThanked = true;
            GameObject.Find("CastElement").GetComponent<ToolTipTrigger>().otherCondition = true;
        }
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Advances to the next dialogue in the list, looping or clamping as appropriate.
=======
    /// Advances the dialogue index, looping or clamping based on dialogueDictionary.
>>>>>>> Stashed changes
    /// </summary>
    public override void IncNextDialogue()
    {
        nextDialogue++;
        if (nextDialogue >= basic_dialogues_list.Count)
        {
            if (dialogueDictionary[basic_dialogues_list])
            {
                nextDialogue = 0;
            }
            else
            {
                nextDialogue = Mathf.Max(0, basic_dialogues_list.Count - 1);
            }
        }
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Unity FixedUpdate method. Handles the King's status and movement logic.
=======
    /// Unity FixedUpdate method. Handles movement and walk pattern logic.
>>>>>>> Stashed changes
    /// </summary>
    void FixedUpdate()
    {
        ActOnStatus();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the King's behavior based on current state (idle or walking).
=======
    /// Handles King's behavior based on current state (Idle or Walking).
    /// Includes raycast logic for blocking and wall detection.
>>>>>>> Stashed changes
    /// </summary>
    public override void ActOnStatus()
    {
        float rayLength = 1.5f;
        if (movement.y < 0 && movement.x == 0)
        {
            rayLength = 0.6f;
        }
        LayerMask mask = LayerMask.GetMask("Player", "Walls");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, rayLength, mask);
        if (currentState == NPCStatus.Walking && hit && hit.collider.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Blocked", true);
            return;
        }
        else if (!hit || (hit && !hit.collider.gameObject.CompareTag("Player")))
        {
            animator.SetBool("Blocked", false);
        }
        if (currentState == NPCStatus.Walking && hit &&
            hit.transform.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            currentWalkProgress += walkDist;
        }

        if (currentState == NPCStatus.Idle)
        {
            OnIdleStatus();
        }
        else if (currentState == NPCStatus.Walking)
        {
            OnWalkStatus();
            DoMovement();
        }
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the King's walking logic and selects the appropriate walk pattern.
=======
    /// Handles walk pattern selection and delegates to the appropriate walk pattern method.
>>>>>>> Stashed changes
    /// </summary>
    public void OnWalkStatus()
    {
        if (movement.sqrMagnitude < 0.1f)
        {
            SetWalkPattern(currentWalkPattern);
        }

<<<<<<< Updated upstream
        // Walk patterns Behaviour
=======
        // Walk patterns behaviour
>>>>>>> Stashed changes
        if (currentWalkPattern == WalkPattern.VLine)
        {
            OnWalkVline();
        }
        else if (currentWalkPattern == WalkPattern.HLine)
        {
            OnWalkHline();
        }
        else if (currentWalkPattern == WalkPattern.Circle)
        {
            OnWalkCircle();
        }
        else if (currentWalkPattern == WalkPattern.Destination)
        {
            OnWalkDestination();
        }
        else if (currentWalkPattern == WalkPattern.Random)
        {
            OnWalkRandom();
        }
        else if (currentWalkPattern == WalkPattern.MultipleDestinations)
        {
            OnWalkMultipleDestinations();
        }
    }

    /// <summary>
    /// Handles walking to multiple destinations in sequence.
    /// </summary>
    public void OnWalkMultipleDestinations()
    {
        MovementToDestination(destinations[destinationsIndex]);
        if (movement.sqrMagnitude < 0.1f)
        {
            destinationsIndex += 1;
            if (destinationsIndex >= destinations.Count)
            {
                destinationsIndex = 0;
            }
        }
    }

    /// <summary>
    /// Handles horizontal line walking pattern.
    /// </summary>
    public void OnWalkHline()
    {
        if (movement.x == 0)
        {
            movement.x = 1;
        }
        if (movement.y != 0)
        {
            movement.y = 0;
        }
        if (currentWalkProgress >= walkDist)
        {
            currentWalkProgress = 0;
            movement.x *= -1;
        }
    }

    /// <summary>
    /// Handles vertical line walking pattern.
    /// </summary>
    public void OnWalkVline()
    {
        if (movement.y == 0)
        {
            movement.y = 1;
        }
        if (movement.x != 0)
        {
            movement.x = 0;
        }
        if (currentWalkProgress >= walkDist)
        {
            currentWalkProgress = 0;
            movement.y *= -1;
        }
    }

    /// <summary>
    /// Handles circular walking pattern.
    /// </summary>
    public void OnWalkCircle()
    {
        if ((movement.x == 0f && movement.y == 0f) || (Mathf.Abs(movement.x) > 0.1f && Mathf.Abs(movement.y) > 0.1f))
        {
            movement.y = 1f;
            movement.x = 0f;
        }
        if (currentWalkProgress >= walkDist)
        {
            currentWalkProgress = 0;
            movement = Quaternion.Euler(0f, 0f, 90f) * movement;
            movement.x = Mathf.Round(movement.x);
            movement.y = Mathf.Round(movement.y);
        }
    }

    /// <summary>
    /// Handles walking to a single destination.
    /// </summary>
    public void OnWalkDestination()
    {
        MovementToDestination(destination);
        if (movement.sqrMagnitude < 0.1f)
        {
            ChangeState(NPCStatus.Idle);
            currentWalkProgress = 0f;
        }
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles random walking pattern, including bounds checking.
=======
    /// Handles random walking pattern within bounds.
>>>>>>> Stashed changes
    /// </summary>
    public void OnWalkRandom()
    {
        // Check if goes out of bounds
<<<<<<< Updated upstream
        if (currentWalkPattern != WalkPattern.Destination &&
            (transform.position.x > walkMaxLimit.x || transform.position.y > walkMaxLimit.y ||
             transform.position.x < walkMinLimit.x || transform.position.y < walkMinLimit.y))
=======
        if (currentWalkPattern != WalkPattern.Destination && (transform.position.x > walkMaxLimit.x || transform.position.y > walkMaxLimit.y || transform.position.x < walkMinLimit.x || transform.position.y < walkMinLimit.y))
>>>>>>> Stashed changes
        {
            movement.x = homePosition.x - transform.position.x;
            movement.y = homePosition.y - transform.position.y;
            return;
        }
        if (currentWalkProgress >= walkDist)
        {
            currentWalkProgress = 0f;
            if (Random.Range(0, 2) == 0)
            {
                movement = new Vector2(1, 0);
                if (Random.Range(0, 2) == 0)
                {
                    movement.x *= -1;
                }
            }
            else
            {
                movement = new Vector2(0, 1);
                if (Random.Range(0, 2) == 0)
                {
                    movement.y *= -1;
                }
            }
        }
    }

    /// <summary>
    /// Moves the King based on the current movement vector and speed.
    /// </summary>
    public void DoMovement()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetBool("Walking", true);

        movement.Normalize();

        // Moves the Rigidbody2D based on the current movement vector and speed.
        Vector2 moving_by = GetMovement() * GetWalkSpeed() * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moving_by);
        currentWalkProgress += moving_by.magnitude;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles idle status by stopping walking animation.
=======
    /// Handles idle state logic (sets animation).
>>>>>>> Stashed changes
    /// </summary>
    public void OnIdleStatus()
    {
        animator.SetBool("Walking", false);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Calculates the movement vector towards a goal position.
=======
    /// Sets the movement vector towards a goal position.
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="goal">The target position.</param>
    public void MovementToDestination(Vector2 goal)
    {
        movement = goal - new Vector2(transform.position.x, transform.position.y);
    }

    /// <summary>
    /// Gets the current movement vector.
    /// </summary>
    /// <returns>The current movement vector.</returns>
    public Vector2 GetMovement()
    {
        return movement;
    }

    /// <summary>
    /// Sets the current walk pattern and resets progress.
    /// </summary>
    /// <param name="newpattern">The new walk pattern.</param>
    public void SetWalkPattern(WalkPattern newpattern)
    {
        currentWalkProgress = 0f;
        currentWalkPattern = newpattern;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Sets the destination and switches to walking state.
=======
    /// Sets the destination for Destination walk pattern and starts walking.
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="newDestination">The new destination.</param>
    public void SetDestination(Vector2 newDestination)
    {
        destination = newDestination;
        ChangeState(NPCStatus.Walking);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Sets the walk speed.
    /// </summary>
    /// <param name="newSpeed">The new walk speed.</param>
=======
    /// Sets the King's walking speed.
    /// </summary>
    /// <param name="newSpeed">The new walking speed.</param>
>>>>>>> Stashed changes
    public void SetWalkSpeed(float newSpeed)
    {
        walkSpeed = newSpeed;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Sets the walk distance.
=======
    /// Sets the King's walk distance before changing direction.
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="newDist">The new walk distance.</param>
    public void SetWalkDist(float newDist)
    {
        walkDist = newDist;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Gets the walk speed.
    /// </summary>
    /// <returns>The walk speed.</returns>
=======
    /// Gets the King's walking speed.
    /// </summary>
    /// <returns>The current walking speed.</returns>
>>>>>>> Stashed changes
    public float GetWalkSpeed()
    {
        return walkSpeed;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Gets the walk distance.
    /// </summary>
    /// <returns>The walk distance.</returns>
=======
    /// Gets the King's walk distance before changing direction.
    /// </summary>
    /// <returns>The current walk distance.</returns>
>>>>>>> Stashed changes
    public float GetWalkDist()
    {
        return walkDist;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Sets the movement vector.
=======
    /// Sets the current movement vector directly.
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="newMovement">The new movement vector.</param>
    public void SetMovement(Vector2 newMovement)
    {
        movement = newMovement;
    }

<<<<<<< Updated upstream
=======
    /// <summary>
    /// Context menu: sets the King's state to Walking.
    /// </summary>
>>>>>>> Stashed changes
    [ContextMenu("Currentstate Walk")]
    void switchWalk()
    {
        ChangeState(NPCStatus.Walking);
    }

<<<<<<< Updated upstream
=======
    /// <summary>
    /// Context menu: sets the King's state to Idle.
    /// </summary>
>>>>>>> Stashed changes
    [ContextMenu("Currentstate Idle")]
    void switchIdle()
    {
        ChangeState(NPCStatus.Idle);
    }

<<<<<<< Updated upstream
    // The following methods are for testing and are not part of the gameplay logic.
    // They do not require Doxygen comments, but a simple C# docstring is helpful for clarity.

    /// <summary>
    /// For testing: calls Start().
=======
    /// <summary>
    /// For testing: calls Start() manually.
>>>>>>> Stashed changes
    /// </summary>
    public void TestCallStart()
    {
        Start();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// For testing: calls FixedUpdate().
=======
    /// For testing: calls FixedUpdate() manually.
>>>>>>> Stashed changes
    /// </summary>
    public void TestCallFixedUpdate()
    {
        FixedUpdate();
    }
}
