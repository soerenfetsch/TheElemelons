using UnityEngine;

/// <summary>
/// Represents the paused state of the game.
/// Handles logic for pausing gameplay.
/// </summary>
public class PauseState : IGameState
{
    /// <summary>
    /// Called when entering the pause state.
    /// Typically sets the game to a paused condition (e.g., Time.timeScale = 0) and displays the pause menu.
    /// </summary>
    public void Enter()
    {
        Logger.Log("Entering Pause State");
        Time.timeScale = 0f;
        // Example: UIManager.Instance.ShowPauseMenuPanel();
    }

    /// <summary>
    /// Called when exiting the pause state.
    /// Restores normal time flow and hides the pause menu.
    /// </summary>
    public void Exit()
    {
        Logger.Log("Exiting Pause State");
        Time.timeScale = 1f;
        // Example: UIManager.Instance.HidePauseMenuPanel();
    }

    /// <summary>
    /// Called every frame while paused.
    /// May handle updates specific to the pause menu if needed.
    /// </summary>
    public void UpdateState()
    {
        // Typically empty.
    }
}
