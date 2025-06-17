using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Delegates tutorial tips to the player's tooltip on collision.
/// Can be set to not repeat, to only trigger during certain game progression stages, or to be enabled by an external condition.
/// </summary>
public class ToolTipTrigger : MonoBehaviour
{
    /// <summary>
    /// If true, this trigger can be activated multiple times.
    /// </summary>
    public bool canRepeat = true;

    // Tracks whether this trigger has already been activated (if not repeatable).
    bool wasTriggered = false;

    /// <summary>
    /// The tutorial tip to display when triggered.
    /// </summary>
    public TutorialTip tutorialTip;

    // Reference to the player's ToolTip component.
    ToolTip toolTip;

    /// <summary>
    /// Additional external condition required for this trigger to activate.
    /// </summary>
    public bool otherCondition = true;

    /// <summary>
    /// List of game progression stages during which this trigger is allowed to activate.
    /// </summary>
    public List<GameProgressionStage> gameProgressionConditions;

    /// <summary>
    /// Unity Start method. Finds and caches the player's ToolTip component.
    /// </summary>
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        toolTip = player.GetComponent<ToolTip>();
    }

    /// <summary>
    /// Unity OnTriggerEnter2D method. Triggers the tooltip if all conditions are met.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((canRepeat || !wasTriggered) && otherCondition && other.gameObject.CompareTag("Player") &&
            other.isTrigger && gameProgressionConditions.Contains(GameManager.Instance.CurrentProgressionStage))
        {
            wasTriggered = true;
            toolTip.ChangeCurrentTip(tutorialTip);
        }
    }
}
