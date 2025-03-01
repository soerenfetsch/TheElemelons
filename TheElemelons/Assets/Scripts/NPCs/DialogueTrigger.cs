using UnityEngine;

public class DialogueTrigger : InteractableNPC
{
    [Tooltip("The key used to fetch the DialogueData from DialogueManager.")]
    [SerializeField] private string dialogueKey;

    /// <summary>
    /// This is called when the player interacts with the NPC.
    /// </summary>
    public override void Interact()
    {
        // Start the dialogue using the key
        DialogueManager.Instance.StartDialogue(dialogueKey);
    }

    /// <summary>
    /// Defines how the NPC behaves based on its current state.
    /// </summary>
    public override void ActOnStatus()
    {
        // Example: you might animate or move the NPC based on currentState
        switch (currentState)
        {
            case NPCStatus.Idle:
                // Idle behavior
                break;
            case NPCStatus.Walking:
                // Walking behavior
                break;
        }
    }
}
