using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers and manages dialogue sequences for an NPC when the player interacts.
/// Handles dialogue progression and game state transitions.
/// </summary>
public class DialogueTrigger : InteractableNPC
{
    [Header("Dialogue")]
    protected int nextDialogue = 0;
    public List<DialogueData> basic_dialogues_list;

    /// <summary>
    /// Called by PlayerInteraction when the player presses E near this NPC.
    /// Handles starting and advancing dialogue, and manages game state transitions.
    /// </summary>
    public override void Interact()
    {
        // Set game status to dialogue mode
        GameManager.Instance.ChangeGameStatus(GameStatus.DIALOGUE);

        // If a dialogue is already active, advance it
        if (DialogueManager.Instance.HasDialogue())
        {
            DialogueManager.Instance.AdvanceDialogue();
        }
        else
        {
            // Log when starting a new dialogue sequence for debugging and analytics
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
    /// Advances the dialogue index, looping if necessary.
    /// </summary>
    public virtual void IncNextDialogue()
    {
        nextDialogue = (nextDialogue + 1) % basic_dialogues_list.Count;
    }

    /// <summary>
    /// Handles NPC-specific actions based on status (e.g., animations or behaviors).
    /// </summary>
    public override void ActOnStatus()
    {
        // Example: handle NPC animations or behaviors
    }
}
