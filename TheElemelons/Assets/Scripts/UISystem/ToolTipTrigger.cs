using System.Collections.Generic;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Triggers a specific tutorial tip on the player's ToolTip component upon collision.
/// The trigger behavior can be configured to repeat, only occur during certain game progression stages,
/// or be dependent on an external boolean condition.
=======
/// Delegates tutorial tips to the player's tooltip on collision.
/// Can be set to not repeat, to only trigger during certain game progression stages, or to be enabled by an external condition.
>>>>>>> Stashed changes
/// </summary>
public class ToolTipTrigger : MonoBehaviour
{
    /// <summary>
<<<<<<< Updated upstream
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
=======
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
>>>>>>> Stashed changes
    /// </summary>
    public List<GameProgressionStage> gameProgressionConditions;

    /// <summary>
<<<<<<< Updated upstream
    /// Called when the script instance is being loaded.
    /// Initializes the reference to the player's ToolTip component.
=======
    /// Unity Start method. Finds and caches the player's ToolTip component.
>>>>>>> Stashed changes
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
