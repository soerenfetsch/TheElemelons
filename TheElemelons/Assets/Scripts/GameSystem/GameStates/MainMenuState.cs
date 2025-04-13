using UnityEngine;

/// <summary>
/// Represents the main menu state. Activates main menu UI and related logic.
/// </summary>
public class MainMenuState : IGameState
{
    /// <summary>
    /// Called when entering the main menu state.
    /// Activates main menu UI and may play background music.
    /// </summary>
    public void Enter()
    {
        Logger.Log("Entering Main Menu State");
        // Example: UIManager.Instance.ShowMainMenuPanel();
    }

    /// <summary>
    /// Called when exiting the main menu state.
    /// Hides main menu UI.
    /// </summary>
    public void Exit()
    {
        Logger.Log("Exiting Main Menu State");
        // Example: UIManager.Instance.HideMainMenuPanel();
    }

    /// <summary>
    /// Called once per frame while in the main menu.
    /// Main menu usually does not require continuous updates.
    /// </summary>
    public void UpdateState()
    {
        // Typically empty.
    }
}
