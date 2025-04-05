using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Components.DictionaryAdapter.Xml;
using UnityEditor;
using UnityEngine;

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
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        ChangeState(NPCStatus.Walking);
    }
    void Update()
    {
        ActOnStatus();
    }
    public override void ActOnStatus()
    {
        float rayLength = 1.5f;
        if (movement.y < 0 && movement.x == 0)
        {
            rayLength = 0.6f;
        }
        LayerMask mask = LayerMask.GetMask("Player", "Walls");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, rayLength, mask);
        UnityEngine.Debug.DrawRay(transform.position, movement * rayLength, Color.yellow);
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
            UnityEngine.Debug.Log("wall");
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
    public void OnWalkStatus()
    {
        if (movement.sqrMagnitude < 0.1f)
        {
            SetWalkPattern(currentWalkPattern);
        }

        //Walk patterns Behaviour
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
    public void OnWalkDestination()
    {
        MovementToDestination(destination);
        if (movement.sqrMagnitude < 0.1f)
        {
            ChangeState(NPCStatus.Idle);
            currentWalkProgress = 0f;
        }
    }
    public void OnWalkRandom()
    {
        //Check if goes out of bounds
        if (currentWalkPattern != WalkPattern.Destination && (transform.position.x > walkMaxLimit.x || transform.position.y > walkMaxLimit.y || transform.position.x < walkMinLimit.x || transform.position.y < walkMinLimit.y))
        {
            UnityEngine.Debug.Log("out of bounds");
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
    public void DoMovement()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetBool("Walking", true);

        movement.Normalize();

        // Moves the Rigidbody2D based on the current movement vector and speed.
        Vector2 moving_by = GetMovement() * GetWalkSpeed() * Time.deltaTime;
        rb.MovePosition(rb.position + moving_by);
        currentWalkProgress += moving_by.magnitude;

    }
    public void OnIdleStatus()
    {
        animator.SetBool("Walking", false);
    }
    public void MovementToDestination(Vector2 goal)
    {
        movement = goal - new Vector2(transform.position.x, transform.position.y);
    }
    public Vector2 GetMovement()
    {
        return movement;
    }
    public void SetWalkPattern(WalkPattern newpattern)
    {
        currentWalkProgress = 0f;
        currentWalkPattern = newpattern;
    }
    public void SetDestination(Vector2 newDestination)
    {
        destination = newDestination;
        ChangeState(NPCStatus.Walking);
    }
    public void SetWalkSpeed(float newSpeed)
    {
        walkSpeed = newSpeed;
    }
    public void SetWalkDist(float newDist)
    {
        walkDist = newDist;
    }
    public float GetWalkSpeed()
    {
        return walkSpeed;
    }
    public float GetWalkDist()
    {
        return walkDist;
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




}
