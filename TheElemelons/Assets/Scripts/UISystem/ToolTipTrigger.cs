using System.Collections;
using System.Collections.Generic;
using Ionic.BZip2;
using UnityEngine;

/// <summary>
/// Triggers a specific tutorial tip on the player's ToolTip component upon collision.
/// The trigger behavior can be configured to repeat, only occur during certain game progression stages,
/// or be dependent on an external boolean condition.
/// </summary>
public class ToolTipTrigger : MonoBehaviour
{
    /// <summary>
    /// If true, the trigger can activate multiple times. If false, it only activates once.
    /// </summary>
    public bool canRepeat = true;
    /// <summary>
    /// Tracks whether this trigger has been activated at least once. Used when canRepeat is false.
    /// </summary>
    bool wasTriggered = false;
    /// <summary>
    /// The type of <see cref="TutorialTip"/> to display when this trigger is activated.
    /// </summary>
    public TutorialTip tutorialTip;
    /// <summary>
    /// Reference to the <see cref="ToolTip"/> component on the player GameObject.
    /// </summary>
    ToolTip toolTip;
    /// <summary>
    /// An additional boolean condition that must be true for the trigger to activate.
    /// Can be controlled by other scripts.
    /// </summary>
    public bool otherCondition = true;
    /// <summary>
    /// A list of <see cref="GameProgressionStage"/>s during which this trigger is allowed to activate.
    /// If empty, the trigger is not restricted by game progression.
    /// </summary>
    public List<GameProgressionStage> gameProgressionConditions;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the reference to the player's ToolTip component.
    /// </summary>
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        toolTip = player.GetComponent<ToolTip>();
    }
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
