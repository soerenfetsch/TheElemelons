using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Start a dialogue on collision.
/// Can be set to not repeat, to only trigger during a certain game progression stage, or to be enabled by external condition.
/// </summary>
public class DialogOnWalkInTrigger : DialogueTrigger
{
    /// <summary>
    /// Whether the dialogue can currently be triggered by the player.
    /// </summary>
    public bool canInteract = true;

    /// <summary>
    /// Whether the dialogue can be triggered repeatedly.
    /// </summary>
    public bool canRepeat = true;

    // Tracks if the dialogue was already triggered (for non-repeating triggers).
    bool wasTriggered = false;

    /// <summary>
    /// List of progression stages during which the dialogue can be triggered.
    /// </summary>
    public List<GameProgressionStage> canInteractCondition;

    /// <summary>
    /// Cooldown time (in seconds) before the dialogue can be triggered again.
    /// </summary>
    public float dialogueCooldown = 60f;

    /// <summary>
    /// Additional external condition that must be true for the dialogue to trigger.
    /// </summary>
    public bool otherCondition = true; // set externally

    /// <summary>
    /// Override for Interact. Not used in this trigger.
    /// </summary>
    public override void Interact()
    { }

    /// <summary>
    /// Triggers the dialogue and starts the cooldown.
    /// </summary>
    public virtual void DoDialog()
    {
        base.Interact();
        StartCoroutine(DialogueCooldown());
    }

    // Coroutine to handle the cooldown between dialogue triggers.
    IEnumerator DialogueCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(dialogueCooldown);
        canInteract = true;
    }

    // Called when another collider enters the trigger collider attached to this object.
    // If the collider is the player and all conditions are met, triggers the dialogue.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (canInteract && other.gameObject.CompareTag("Player") &&
            other.isTrigger && (canRepeat || !wasTriggered) && otherCondition &&
            canInteractCondition.Contains(GameManager.Instance.CurrentProgressionStage))
        {
            DoDialog();
            wasTriggered = true;
        }
    }
}
