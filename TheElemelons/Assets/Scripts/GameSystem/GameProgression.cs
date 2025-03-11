using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject that holds the player's progression data.
/// This includes the current dungeon, collected Elemelons, completed dialogues,
/// and the player's stats such as health and mana.
/// </summary>
[CreateAssetMenu(fileName = "GameProgress", menuName = "Game System/Progression")]
public class GameProgression : ScriptableObject
{
    #region Public Fields

    // TODO: MapData

    /// <summary>
    /// The index of the current dungeon or level.
    /// </summary>
    public int currentDungeon = 0;

    /// <summary>
    /// List of identifiers for collected Elemelons.
    /// </summary>
    public List<string> collectedElemelons = new List<string>();

    /// <summary>
    /// List of dialogue keys that have been completed.
    /// </summary>
    public List<string> completedDialogues = new List<string>();

    /// <summary>
    /// The player's current or maximum health.
    /// </summary>
    public int playerHealth = 3;

    /// <summary>
    /// The player's current or maximum mana.
    /// </summary>
    public int playerMana = 3;

    #endregion

    #region Public Methods

    /// <summary>
    /// Resets all progression data to default values.
    /// Clears lists of collected items and completed dialogues,
    /// and resets dungeon, health, and mana values.
    /// </summary>
    public void ResetProgress()
    {
        currentDungeon = 0;
        collectedElemelons.Clear();
        completedDialogues.Clear();
        playerHealth = 3;
        playerMana = 3;
    }

    /// <summary>
    /// Marks a dialogue as completed by adding its key to the list,
    /// if it hasn't been marked already.
    /// </summary>
    /// <param name="dialogueKey">The unique key of the dialogue to mark as completed.</param>
    public void MarkDialogueCompleted(string dialogueKey)
    {
        if (!completedDialogues.Contains(dialogueKey))
        {
            completedDialogues.Add(dialogueKey);
        }
    }

    #endregion
}