using UnityEngine;

/// <summary>
/// Visual effect for the confused debuff. Attaches a question mark above the affected entity's head.
/// </summary>
public class QuestionMark : MonoBehaviour
{
    Collider2D parentCollider;
    Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("question mark");
        InitializeParent();
    }

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

    void FixedUpdate()
    {
        if (parent == null)
        {
            InitializeParent();
        }
        transform.position = new Vector3(transform.position.x, parentCollider.bounds.max.y + 0.5f, 0);
    }
}
