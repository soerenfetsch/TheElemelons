/// <summary>
/// Enum representing the current status or mode of the game.
/// Used to control input, UI, and game logic flow.
/// </summary>
public enum GameStatus
{
    /// <summary>
    /// The player is actively playing the game.
    /// </summary>
    INGAME,
    /// <summary>
    /// The main menu is open.
    /// </summary>
    MAIN_MENU,
    /// <summary>
    /// The pause menu is open.
    /// </summary>
    PAUSE_MENU,
    /// <summary>
    /// A dialogue sequence is active.
    /// </summary>
    DIALOGUE,
    /// <summary>
    /// The settings menu is open.
    /// </summary>
    SETTINGS_MENU,
    /// <summary>
    /// The death screen is displayed (player has died).
    /// </summary>
    DEATH_SCREEN,
    /// <summary>
    /// The loading screen is displayed (scene or data loading).
    /// </summary>
    LOADING_SCREEN
}