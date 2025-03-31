using System.Collections;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter.Xml;
using UnityEditor;
using UnityEngine;

public enum WalkPattern
{
    Circle,
    HLine,
    VLine,
    Random,
    Destination
}
public class King : DialogueTrigger
{
    [Header("Walk Patterns")]
    public float walkDist = 3f;
    float currentWalkProgress = 0f;
    public float walkSpeed = 1.5f;
    private float walkTime;
    public WalkPattern currentWalkPattern = WalkPattern.Circle;
    public Vector2 destination = Vector2.zero;
    public Vector2 walkMaxLimit;
    public Vector2 walkMinLimit;
    public Vector2 homePosition;
    int turnCount = 0;
    [Header("Movement")]
    private Rigidbody2D rb;
    private Animator animator;
    public Vector2 movement;
    [Header("Interact")]
    private Transform playerTransform;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        playerTransform = GameObject.Find("Player").transform;
        UpdateWalkTime();
        ChangeState(NPCStatus.Walking);
        StartCoroutine(UpdateTurnCount());
    }

    void Update()
    {
        ActOnStatus();
    }

    public override void ActOnStatus()
    {
        float rayLength = 1.5f;
        if (movement.y < 0)
        {
            rayLength = 0.6f;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, rayLength);
        Debug.DrawRay(transform.position, movement * rayLength, Color.yellow);
        if (currentState == NPCStatus.Walking && hit && hit.collider.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Blocked", true);
            return;
        }
        else if (!hit || (hit && !hit.collider.CompareTag("Player")))
        {
            animator.SetBool("Blocked", false);
        }
        if (currentState == NPCStatus.Walking && hit && hit.rigidbody != null &&
        hit.transform.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            currentWalkProgress += walkDist;
            Debug.Log("wall");
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
            Debug.Log("next move: " + movement.ToString());
        }
    }
    public void OnWalkDestination()
    {
        movement = destination - new Vector2(transform.position.x, transform.position.y);
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
            Debug.Log("out of bounds");
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
        Vector2 moving_by = rb.position + GetMovement() * GetWalkSpeed() * Time.deltaTime;
        rb.MovePosition(moving_by);
        currentWalkProgress += moving_by.magnitude;

    }
    public void OnIdleStatus()
    {
        animator.SetBool("Walking", false);
    }
    public void InteractMethod()
    { }
    public Vector2 GetMovement()
    {
        return movement;
    }
    public void SetWalkPattern(WalkPattern newpattern)
    {
        currentWalkProgress = 0f;
        currentWalkPattern = newpattern;
        turnCount = 0;
    }
    public void SetDestination(Vector2 newDestination)
    {
        destination = newDestination;
        ChangeState(NPCStatus.Walking);
    }
    IEnumerator UpdateTurnCount()
    {
        float waitChunk = 0.1f;
        while (true)
        {
            float timer = 0;
            while (timer < walkTime)
            {
                yield return new WaitForSeconds(waitChunk);
                timer += waitChunk;
            }
            turnCount++;
        }
    }
    public void UpdateWalkTime()
    {
        walkTime = walkDist / walkSpeed;
    }
    public void SetWalkSpeed(float newSpeed)
    {
        walkSpeed = newSpeed;
        UpdateWalkTime();
    }
    public void SetWalkDist(float newDist)
    {
        walkDist = newDist;
        UpdateWalkTime();
    }
    public float GetWalkSpeed()
    {
        return walkSpeed;
    }
    public float GetWalkDist()
    {
        return walkDist;
    }
    public float GetWalkTime()
    {
        return walkTime;
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
