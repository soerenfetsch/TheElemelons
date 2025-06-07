using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents the data structure used to save and load the player's game state,
/// including progression, stats, save time, and key bindings.
/// </summary>
[System.Serializable]
public class GameSaveData
{
    /// <summary>
    /// Represents a single key binding entry for an action and its assigned key code.
    /// </summary>
    [System.Serializable]
    public class KeyBindingEntry
    {
        /// <summary>
        /// The name of the action (e.g., "Jump", "Attack").
        /// </summary>
        public string action;

        /// <summary>
        /// The key code assigned to the action, stored as an integer.
        /// </summary>
        public int keyCode; // store as int for KeyCode
    }

    /// <summary>
    /// The current progression stage of the game.
    /// </summary>
    public GameProgressionStage progressionStage;

    /// <summary>
    /// The maximum health value for the player at the time of saving.
    /// </summary>
    public int maxHealth = 3;

    /// <summary>
    /// The maximum mana value for the player at the time of saving.
    /// </summary>
    public int maxMana = 3;

    /// <summary>
    /// The date and time when the game was saved.
    /// </summary>
    public DateTime saveDate;

    /// <summary>
    /// The list of key bindings for player actions.
    /// </summary>
    public List<KeyBindingEntry> keyBindings = new List<KeyBindingEntry>(); // ADD THIS
}