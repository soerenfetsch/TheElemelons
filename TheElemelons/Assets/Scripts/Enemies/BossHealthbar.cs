using UnityEngine;

/// <summary>
/// Controls the boss health bar UI, updating its position and appearance based on the boss's current health.
/// </summary>
public class BossHealthbar : MonoBehaviour
{
    /// <summary>
    /// Speed at which the blood marker moves to its new position.
    /// </summary>
    public float moveSpeed = 2f;

    /// <summary>
    /// UI element representing the current health (the blood marker).
    /// </summary>
    public RectTransform bloodMarker;

    /// <summary>
    /// UI element representing the end of the health bar (zero health).
    /// </summary>
    public RectTransform endOfLife;

    /// <summary>
    /// Reference to the boss's Health component.
    /// </summary>
    public Health bossHealth;

    // Starting position of the blood marker.
    Vector2 startPos;
    // Distance between the start and end of the health bar.
    float dist;
    // Current destination X position for the blood marker.
    float destination;

    /// <summary>
    /// Initializes the health bar on start.
    /// </summary>
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Updates the health bar position and checks for boss health validity each frame.
    /// </summary>
    void Update()
    {
        CalcDestination();
        MoveToDestination();
        if (bossHealth == null || bossHealth.gameObject == null)
        {
            ResetBloodMarker();
        }
    }

    /// <summary>
    /// Resets the blood marker when the health bar is enabled.
    /// </summary>
    void OnEnable()
    {
        ResetBloodMarker();
    }

    /// <summary>
    /// Resets the blood marker to its initial position.
    /// </summary>
    public void ResetBloodMarker()
    {
        bloodMarker.localPosition = new Vector3(435, 19, 0);
    }

    /// <summary>
    /// Initializes the health bar's start position and distance.
    /// </summary>
    void Initialize()
    {
        startPos = bloodMarker.position;
        dist = bloodMarker.position.x - endOfLife.position.x;
        destination = bloodMarker.position.y;
    }

    /// <summary>
    /// Calculates the new destination for the blood marker based on the boss's current health.
    /// </summary>
    void CalcDestination()
    {
        float percentage = bossHealth.health / bossHealth.maxhealth;
        destination = endOfLife.position.x + dist * percentage;
        if (bossHealth == null)
        {
            destination = endOfLife.position.x;
        }
    }

    /// <summary>
    /// Moves the blood marker towards the calculated destination.
    /// </summary>
    void MoveToDestination()
    {
        if (bloodMarker.position.x - destination > 0.1f)
        {
            Vector2 movement = new Vector2(-1, 0) * moveSpeed * Time.deltaTime;
            bloodMarker.position = bloodMarker.position + (Vector3)movement;
        }
    }
}
