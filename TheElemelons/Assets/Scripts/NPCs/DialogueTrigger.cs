using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for NPCs or objects that can trigger dialogue sequences.
/// Handles dialogue selection, cycling, and interaction logic with the player.
/// </summary>
public class DialogueTrigger : InteractableNPC
{
    /// <summary>
    /// Index of the next dialogue to trigger from <see cref="basic_dialogues_list"/>.
    /// </summary>
    [Header("Dialogue")]
    protected int nextDialogue = 0;

    /// <summary>
    /// List of dialogue data assets available for this trigger.
    /// </summary>
    public List<DialogueData> basic_dialogues_list;

    /// <summary>
    /// Called when the player interacts with this NPC or object.
    /// Handles starting or advancing dialogue, and manages dialogue state.
    /// </summary>
    public override void Interact()
    {
        // Set game status to dialogue mode
        GameManager.Instance.ChangeGameStatus(GameStatus.DIALOGUE);
        Debug.Log($"[DialogueTrigger] Interact called on {gameObject.name}. nextDialogue index: {nextDialogue}. HasDialogue: {DialogueManager.Instance != null && DialogueManager.Instance.HasDialogue()}");

        if (GameManager.Instance == null)
        {
            Debug.LogError("[DialogueTrigger] GameManager.Instance is NULL in Interact!");
            return;
        }
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[DialogueTrigger] DialogueManager.Instance is NULL in Interact!");
            return;
        }

        // If a dialogue is already active, advance it; otherwise, start a new dialogue sequence.
        if (DialogueManager.Instance.HasDialogue())
        {
            DialogueManager.Instance.AdvanceDialogue();
        }
        else
        {
            Debug.Log($"[DialogueTrigger] Starting new dialogue sequence from {gameObject.name}.");

            // If there are no dialogues, revert game status and exit
            if (basic_dialogues_list == null || basic_dialogues_list.Count == 0)
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.INGAME); // Revert status if dialogue can't start
                Debug.LogWarning("[DialogueTrigger] No dialogues found for this NPC."); // Log warning for missing data
                return;
            }

            DialogueData dataToStart = basic_dialogues_list[nextDialogue];
            if (dataToStart == null)
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
                Debug.LogWarning($"[DialogueTrigger] DialogueData at index {nextDialogue} is null."); // Log warning for null dialogue
                IncNextDialogue(); // Try to advance for next time, or clamp
                return;
            }

            DialogueManager.Instance.StartDialogue(dataToStart);

            IncNextDialogue();
        }
    }

    /// <summary>
    /// Advances the <see cref="nextDialogue"/> index, cycling through the dialogue list.
    /// Can be overridden for custom dialogue cycling logic.
    /// </summary>
    public virtual void IncNextDialogue()
    {
        nextDialogue = (nextDialogue + 1) % basic_dialogues_list.Count;
    }

    /// <summary>
    /// Called to handle NPC-specific status updates (e.g., animations or behaviors).
    /// Can be overridden in derived classes.
    /// </summary>
    public override void ActOnStatus()
    {
        // Example: handle NPC animations or behaviors
    }
}
