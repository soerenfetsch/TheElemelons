using UnityEngine;

/// <summary>
/// Represents the dialogue state of the game.
/// In this state, the game may pause or restrict player movement,
/// and the dialogue UI is activated so that the player can interact with dialog content.
/// </summary>
public class DialogueState : IGameState
{
    /// <summary>
    /// Called when entering the dialogue state.
    /// Ideally, this should disable gameplay controls and activate the dialogue UI.
    /// </summary>
    public void Enter()
    {
        Logger.Log("Entering Dialogue State");
        // Optionally, disable player movement or pause time if desired:
        // e.g., Time.timeScale = 0f;
        // The UIManager should handle showing the dialogue panel via its mapping.
    }

    /// <summary>
    /// Called when exiting the dialogue state.
    /// This should re-enable gameplay controls and hide the dialogue UI.
    /// </summary>
    public void Exit()
    {
        Logger.Log("Exiting Dialogue State");
        // Re-enable game controls:
        // e.g., Time.timeScale = 1f;
    }

    /// <summary>
    /// Called every frame while in dialogue.
    /// If needed, add logic for auto-advancing dialogue text here.
    /// </summary>
    public void UpdateState()
    {
        // Typically, dialogue doesn't require per-frame updates,
        // unless you implement auto-advance or dynamic effects.
    }
}
