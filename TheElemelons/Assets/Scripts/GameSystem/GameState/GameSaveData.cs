using System;
using System.Collections.Generic;

/// <summary>
/// Serializable class representing all data needed to save and load game progress.
/// Includes progression stage, player stats, save date, and key bindings.
/// </summary>
[System.Serializable]
public class GameSaveData
{
    /// <summary>
    /// Serializable entry for a single key binding (action and key code).
    /// </summary>
    [System.Serializable]
    public class KeyBindingEntry
    {
        /// <summary>
        /// The action name (e.g., "MoveLeft", "CastSpell").
        /// </summary>
        public string action;
        /// <summary>
        /// The key code for the action, stored as an integer.
        /// </summary>
        public int keyCode; // store as int for KeyCode
    }

    /// <summary>
    /// The current progression stage of the game.
    /// </summary>
    public GameProgressionStage progressionStage;

    /// <summary>
    /// The player's maximum health at the time of saving.
    /// </summary>
    public int maxHealth = 3;

    /// <summary>
    /// The player's maximum mana at the time of saving.
    /// </summary>
    public int maxMana = 3;

    /// <summary>
    /// The date and time when the game was saved.
    /// </summary>
    public DateTime saveDate;

    /// <summary>
    /// List of key binding entries representing the player's custom key bindings.
    /// </summary>
    public List<KeyBindingEntry> keyBindings = new List<KeyBindingEntry>();
}