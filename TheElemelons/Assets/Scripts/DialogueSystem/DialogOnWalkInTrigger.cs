using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Start a dialogue on collision
/// can be set to not repeat, to only trigger during a certain game progression stage or to be enabled by external condition
/// </summary>
public class DialogOnWalkInTrigger : DialogueTrigger
{
    public bool canInteract = true;
    public bool canRepeat = true;
    bool wasTriggered = false;
    public List<GameProgressionStage> canInteractCondition;
    public float dialogueCooldown = 60f;
    public bool otherCondition = true; // set externally

    /// <summary>
    /// Called when the player interacts with the trigger. (Override does nothing by default.)
    /// </summary>
    public override void Interact()
    { }

    /// <summary>
    /// Initiates the dialogue and starts the cooldown coroutine.
    /// </summary>
    public virtual void DoDialog()
    {
        base.Interact();
        StartCoroutine(DialogueCooldown());
    }

    /// <summary>
    /// Coroutine that disables interaction for the duration of the cooldown.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator DialogueCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(dialogueCooldown);
        canInteract = true;
    }

    /// <summary>
    /// Unity event called when another collider enters the trigger.
    /// Starts the dialogue if all conditions are met.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
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
