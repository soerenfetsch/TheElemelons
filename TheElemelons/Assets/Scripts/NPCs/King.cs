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
    public float walkSpeed = 1.5f;
    private float walkTime;
    public WalkPattern currentWalkPattern = WalkPattern.Circle;
    public Vector2 destination = Vector2.zero;
    public Vector2 walkMaxLimit;
    public Vector2 walkMinLimit;
    public Vector2 homePosition;
    bool switchBackToPrevPattern = false;
    WalkPattern prevPattern;
    int turnCount = 0;
    [Header("Movement")]
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement);
        if ( currentState == NPCStatus.Walking && hit && hit.transform.CompareTag("Player") && (hit.transform.position - transform.position).sqrMagnitude <1f)
        {
            SaveToPrev();
            currentState = NPCStatus.Idle;
            Debug.Log("detected player");
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
        //Check if goes out of bounds
        if (currentWalkPattern != WalkPattern.Destination && (transform.position.x > walkMaxLimit.x || transform.position.y > walkMaxLimit.y || transform.position.x < walkMinLimit.x || transform.position.y < walkMinLimit.y))
        {
            Debug.Log("out of bounds");
            destination = homePosition;
            SaveToPrev();
            SetWalkPattern(WalkPattern.Destination);
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
            OnWalkCirlce();
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
        if (turnCount == 1)
        {
            turnCount = 0;
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
        if (turnCount != 0)
        {
            turnCount = 0;
            movement.y *= -1;
        }
    }
    public void OnWalkCirlce()
    {
        if ((movement.x == 0f && movement.y == 0f) || (Mathf.Abs(movement.x) > 0.1f && Mathf.Abs(movement.y) > 0.1f))
        {
            turnCount = 0;
            movement.y = 1f;
            movement.x = 0f;
        }
        if (turnCount != 0f)
        {
            turnCount = 0;
            if (movement.x > 0f)
            {
                movement.y = -1f;
                movement.x = 0f;
            }
            else if (movement.y < 0f)
            {
                movement.y = 0f;
                movement.x = -1f;
            } else if (movement.x < 0f)
            {
                movement.y = 1f;
                movement.x = 0f;
            } else if (movement.y > 0f)
            {
                movement.y = 0f;
                movement.x = 1f;
            }
        }
    }
    public void OnWalkDestination()
    {
        movement = destination - new Vector2(transform.position.x, transform.position.y);
        if (movement.sqrMagnitude < 0.1f)
        {
            if (switchBackToPrevPattern)
            {
                SwitchBackToPrevPattern();
            }
            else
            {
                ChangeState(NPCStatus.Idle);
            }

        }
    }
    public void OnWalkRandom()
    {
        if (turnCount == 1)
        {
            turnCount = 0;
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
        rb.MovePosition(rb.position + GetMovement() * GetWalkSpeed() * Time.fixedDeltaTime);

    }
    public void OnIdleStatus()
    {
        animator.SetBool("Walking", false);
    }
    public void InteractMethod()
    { }
    public bool isInfront(Transform obstacle)
    {
        if (movement.x == 1 && obstacle.position.x >= transform.position.x
        || movement.x == -1 && obstacle.position.x <= transform.position.x
        || movement.y == 1 && obstacle.position.y <= transform.position.y
        || movement.y == -1 && obstacle.position.y <= transform.position.y)
        {
            return true;
        }
        return false;
    }
    public Vector2 GetMovement()
    {
        return movement;
    }
    public void SetWalkPattern(WalkPattern newpattern)
    {
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
    public void SaveToPrev()
    {
        if (!switchBackToPrevPattern)
        {
            switchBackToPrevPattern = true;
            prevPattern = currentWalkPattern;
        }
    }
    public void SwitchBackToPrevPattern()
    {
        if (switchBackToPrevPattern)
        {
            currentState = NPCStatus.Walking;
            currentWalkPattern = prevPattern;
            switchBackToPrevPattern = false;
        }

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
