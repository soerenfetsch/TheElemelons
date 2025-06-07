/// <summary>
/// Represents the current state of the game, used to control UI, input, and game logic flow.
/// </summary>
public enum GameStatus
{
    /// <summary>
    /// The game is actively being played.
    /// </summary>
    INGAME,

    /// <summary>
    /// The main menu is displayed.
    /// </summary>
    MAIN_MENU,

    /// <summary>
    /// The pause menu is displayed.
    /// </summary>
    PAUSE_MENU,

    /// <summary>
    /// The game is in a dialogue sequence.
    /// </summary>
    DIALOGUE,

    /// <summary>
    /// The settings menu is displayed.
    /// </summary>
    SETTINGS_MENU,

    /// <summary>
    /// The death screen is displayed after player defeat.
    /// </summary>
    DEATH_SCREEN,

    /// <summary>
    /// The loading screen is displayed during scene or data transitions.
    /// </summary>
    LOADING_SCREEN
}