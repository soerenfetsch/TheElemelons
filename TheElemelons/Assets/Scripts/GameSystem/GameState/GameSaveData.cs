using System;
using System.Collections.Generic;

/// <summary>
<<<<<<< Updated upstream
/// Represents the data structure used to save and load the player's game state,
/// including progression, stats, save time, and key bindings.
=======
/// Serializable class representing all data needed to save and load game progress.
/// Includes progression stage, player stats, save date, and key bindings.
>>>>>>> Stashed changes
/// </summary>
[System.Serializable]
public class GameSaveData
{
    /// <summary>
<<<<<<< Updated upstream
    /// Represents a single key binding entry for an action and its assigned key code.
=======
    /// Serializable entry for a single key binding (action and key code).
>>>>>>> Stashed changes
    /// </summary>
    [System.Serializable]
    public class KeyBindingEntry
    {
        /// <summary>
<<<<<<< Updated upstream
        /// The name of the action (e.g., "Jump", "Attack").
        /// </summary>
        public string action;

        /// <summary>
        /// The key code assigned to the action, stored as an integer.
=======
        /// The action name (e.g., "MoveLeft", "CastSpell").
        /// </summary>
        public string action;
        /// <summary>
        /// The key code for the action, stored as an integer.
>>>>>>> Stashed changes
        /// </summary>
        public int keyCode; // store as int for KeyCode
    }

    /// <summary>
    /// The current progression stage of the game.
    /// </summary>
    public GameProgressionStage progressionStage;

    /// <summary>
<<<<<<< Updated upstream
    /// The maximum health value for the player at the time of saving.
=======
    /// The player's maximum health at the time of saving.
>>>>>>> Stashed changes
    /// </summary>
    public int maxHealth = 3;

    /// <summary>
<<<<<<< Updated upstream
    /// The maximum mana value for the player at the time of saving.
=======
    /// The player's maximum mana at the time of saving.
>>>>>>> Stashed changes
    /// </summary>
    public int maxMana = 3;

    /// <summary>
    /// The date and time when the game was saved.
    /// </summary>
    public DateTime saveDate;

    /// <summary>
<<<<<<< Updated upstream
    /// The list of key bindings for player actions.
    /// </summary>
    public List<KeyBindingEntry> keyBindings = new List<KeyBindingEntry>(); // ADD THIS
=======
    /// List of key binding entries representing the player's custom key bindings.
    /// </summary>
    public List<KeyBindingEntry> keyBindings = new List<KeyBindingEntry>();
>>>>>>> Stashed changes
}