using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Visual effect for the confused debuff. Attaches a question mark above the affected entity's head.
=======
/// Visual effect for the confused debuff.
/// Attaches a question mark above the affected entity's head and follows its movement.
>>>>>>> Stashed changes
/// </summary>
public class QuestionMark : MonoBehaviour
{
    // Reference to the collider of the parent (victim) entity.
    Collider2D parentCollider;
    // Reference to the parent (victim) transform.
    Transform parent;

    // Start is called before the first frame update.
    // Initializes the parent and parent collider references.
    void Start()
    {
        Debug.Log("question mark");
        InitializeParent();
    }

<<<<<<< Updated upstream
=======
    // Finds and assigns the parent transform and its trigger collider.
>>>>>>> Stashed changes
    void InitializeParent()
    {
        if (transform.parent == null)
        {
            FindVictim();
        }
        parent = transform.parent;
        Collider2D[] colliders = parent.GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].isTrigger)
            {
                parentCollider = colliders[i];
            }
        }
    }

<<<<<<< Updated upstream
=======
    // Finds the closest valid victim (player or enemy with Health) within a radius and sets it as the parent.
>>>>>>> Stashed changes
    void FindVictim()
    {
        LayerMask victimMask = LayerMask.GetMask("Enemies", "Player");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3f, victimMask);
        float distance = Mathf.Infinity;
        Collider2D chosen = colliders[0];
        foreach (Collider2D collider in colliders)
        {
            float dist = (transform.position - collider.transform.position).magnitude;
            if (collider.GetComponent<Health>() != null && dist < distance)
            {
                chosen = collider;
                distance = dist;
            }
        }
        if (chosen.GetComponent<Health>() != null)
        {
            transform.parent = chosen.transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }

<<<<<<< Updated upstream
=======
    // FixedUpdate is called at a fixed interval.
    // Keeps the question mark above the parent's head, following its movement.
>>>>>>> Stashed changes
    void FixedUpdate()
    {
        if (parent == null)
        {
            InitializeParent();
        }
        transform.position = new Vector3(transform.position.x, parentCollider.bounds.max.y + 0.5f, 0);
    }
}
