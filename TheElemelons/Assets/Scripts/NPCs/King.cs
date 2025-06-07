using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Components.DictionaryAdapter.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

/// <summary>
/// Represents the King NPC, which can walk in various patterns, interact with the player, and trigger dialogues.
/// Implements both walking and idle behaviors, and manages dialogue progression based on game state.
/// </summary>
public enum WalkPattern
{
    Circle,
    HLine,
    VLine,
    Random,
    Destination,
    MultipleDestinations
}

public class King : DialogueTrigger
{
    [Header("Dialogues")]
    public List<DialogueData> intro;
    public GameObject treeEnemy;
    public List<DialogueData> thankYou;
    public List<DialogueData> standardDialogue;
    public List<DialogueData> endDialogue;
    Dictionary<List<DialogueData>, bool> dialogueDictionary;
    bool alreadyDone = false;
    bool alreadyThanked = false;

    [Header("Walk Patterns")]
    public float walkDist = 3f;
    float currentWalkProgress = 0f;
    public float walkSpeed = 1.5f;
    public WalkPattern currentWalkPattern = WalkPattern.Circle;
    public Vector2 destination = Vector2.zero;
    public List<Vector2> destinations;
    int destinationsIndex = 0;
    public Vector2 walkMaxLimit;
    public Vector2 walkMinLimit;
    public Vector2 homePosition;

    [Header("Movement")]
    private Rigidbody2D rb;
    private Animator animator;
    public Vector2 movement;

    /// <summary>
    /// Unity Start method. Initializes components, sets initial state, and prepares dialogue dictionary.
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
    /// Unity Update method. Updates the King's dialogue list based on the current game progression stage.
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
    /// Handles player interaction with the King. Triggers dialogue and progression changes as needed.
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
    /// Advances to the next dialogue in the list, looping or clamping as appropriate.
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
    /// Unity FixedUpdate method. Handles the King's status and movement logic.
    /// </summary>
    void FixedUpdate()
    {
        ActOnStatus();
    }

    /// <summary>
    /// Handles the King's behavior based on current state (idle or walking).
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
    /// Handles the King's walking logic and selects the appropriate walk pattern.
    /// </summary>
    public void OnWalkStatus()
    {
        if (movement.sqrMagnitude < 0.1f)
        {
            SetWalkPattern(currentWalkPattern);
        }

        // Walk patterns Behaviour
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
    /// Handles random walking pattern, including bounds checking.
    /// </summary>
    public void OnWalkRandom()
    {
        // Check if goes out of bounds
        if (currentWalkPattern != WalkPattern.Destination &&
            (transform.position.x > walkMaxLimit.x || transform.position.y > walkMaxLimit.y ||
             transform.position.x < walkMinLimit.x || transform.position.y < walkMinLimit.y))
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
    /// Handles idle status by stopping walking animation.
    /// </summary>
    public void OnIdleStatus()
    {
        animator.SetBool("Walking", false);
    }

    /// <summary>
    /// Calculates the movement vector towards a goal position.
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
    /// Sets the destination and switches to walking state.
    /// </summary>
    /// <param name="newDestination">The new destination.</param>
    public void SetDestination(Vector2 newDestination)
    {
        destination = newDestination;
        ChangeState(NPCStatus.Walking);
    }

    /// <summary>
    /// Sets the walk speed.
    /// </summary>
    /// <param name="newSpeed">The new walk speed.</param>
    public void SetWalkSpeed(float newSpeed)
    {
        walkSpeed = newSpeed;
    }

    /// <summary>
    /// Sets the walk distance.
    /// </summary>
    /// <param name="newDist">The new walk distance.</param>
    public void SetWalkDist(float newDist)
    {
        walkDist = newDist;
    }

    /// <summary>
    /// Gets the walk speed.
    /// </summary>
    /// <returns>The walk speed.</returns>
    public float GetWalkSpeed()
    {
        return walkSpeed;
    }

    /// <summary>
    /// Gets the walk distance.
    /// </summary>
    /// <returns>The walk distance.</returns>
    public float GetWalkDist()
    {
        return walkDist;
    }

    /// <summary>
    /// Sets the movement vector.
    /// </summary>
    /// <param name="newMovement">The new movement vector.</param>
    public void SetMovement(Vector2 newMovement)
    {
        movement = newMovement;
    }

    [ContextMenu("Currentstate Walk")]
    void switchWalk()
    {
        ChangeState(NPCStatus.Walking);
    }

    [ContextMenu("Currentstate Idle")]
    void switchIdle()
    {
        ChangeState(NPCStatus.Idle);
    }

    // The following methods are for testing and are not part of the gameplay logic.
    // They do not require Doxygen comments, but a simple C# docstring is helpful for clarity.

    /// <summary>
    /// For testing: calls Start().
    /// </summary>
    public void TestCallStart()
    {
        Start();
    }

    /// <summary>
    /// For testing: calls FixedUpdate().
    /// </summary>
    public void TestCallFixedUpdate()
    {
        FixedUpdate();
    }
}
