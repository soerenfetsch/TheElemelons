using UnityEngine;

/// <summary>
/// Triggers dialogue interactions for an NPC. Inherits from <see cref="InteractableNPC"/>.
/// </summary>
public class DialogueTrigger : InteractableNPC
{
    #region Serialized Fields

    /// <summary>
    /// The unique key used to fetch the corresponding DialogueData from the DialogueManager.
    /// </summary>
    [Tooltip("The key used to fetch the DialogueData from DialogueManager.")]
    [SerializeField] private string dialogueKey;

    #endregion

    #region Public Methods

    /// <summary>
    /// Called when the player interacts with this NPC.
    /// Initiates dialogue if the game is in the InGame state and the player is not in combat.
    /// </summary>
    public override void Interact()
    {
        // Ensure dialogue only triggers in normal gameplay.
        if (GameManager.Instance.CurrentState != GameState.InGame)
            return;

        // Check if the player is currently in combat.
        if (GameManager.Instance.isInCombat)
        {
            Debug.Log("Cannot initiate dialogue while in combat!");
            return;
        }

        // Start the dialogue sequence using the DialogueManager.
        DialogueManager.Instance.StartDialogue(dialogueKey);

        // Change the game state to Dialogue so that player input is disabled.
        GameManager.Instance.ChangeGameState(GameState.Dialogue);

        // Mark this dialogue as completed in the player's progression and save the game.
        GameManager.Instance.CurrentProgress.MarkDialogueCompleted(dialogueKey);
        GameSaveProvider.Instance.SaveGame();

        // --- Future Cinematic Dialogue Mode ---
        // In a future version, instead of completely disabling player movement,
        // we might allow a cinematic sequence where the player's controls are disabled,
        // but both the player and the NPC (e.g., the King) can move or perform scripted actions
        // to enhance the narrative. For now, this functionality is not implemented.
    }

    /// <summary>
    /// Defines how the NPC behaves based on its current status.
    /// This method is intended to be customized to animate or move the NPC accordingly.
    /// </summary>
    public override void ActOnStatus()
    {
        // Example behavior based on the NPC's state.
        switch (currentState)
        {
            case NPCStatus.Idle:
                // Idle behavior (e.g., play idle animation).
                break;
            case NPCStatus.Walking:
                // Walking behavior (e.g., follow a predefined path).
                break;
            default:
                break;
        }
    }

    #endregion
}