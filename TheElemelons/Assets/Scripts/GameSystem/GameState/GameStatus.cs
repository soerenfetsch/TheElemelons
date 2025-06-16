/// <summary>
<<<<<<< Updated upstream
/// Represents the current state of the game, used to control UI, input, and game logic flow.
=======
/// Enum representing the current status or mode of the game.
/// Used to control input, UI, and game logic flow.
>>>>>>> Stashed changes
/// </summary>
public enum GameStatus
{
    /// <summary>
<<<<<<< Updated upstream
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
=======
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
>>>>>>> Stashed changes
    /// </summary>
    LOADING_SCREEN
}