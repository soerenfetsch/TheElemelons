using UnityEngine;

/// <summary>
/// Represents the settings state of the game.
/// Displays options for adjusting game configurations.
/// </summary>
public class SettingsState : IGameState
{
    /// <summary>
    /// Called when entering the settings state.
    /// Activates the settings UI.
    /// </summary>
    public void Enter()
    {
        Logger.Log("Entering Settings State");
        // Example: UIManager.Instance.ShowSettingsPanel();
    }

    /// <summary>
    /// Called when exiting the settings state.
    /// Hides the settings UI.
    /// </summary>
    public void Exit()
    {
        Logger.Log("Exiting Settings State");
        // Example: UIManager.Instance.HideSettingsPanel();
    }

    /// <summary>
    /// Called once per frame while in the settings state.
    /// Typically, the settings state may not require per-frame updates.
    /// </summary>
    public void UpdateState()
    {
        // Typically empty.
    }
}
