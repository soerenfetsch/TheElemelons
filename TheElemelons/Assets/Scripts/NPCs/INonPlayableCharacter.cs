using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum defining common NPC states.
/// </summary>
public enum NPCStatus
{
    Idle,
    Walking
}

/// <summary>
/// Interface for non-playable characters (NPCs) in the game.
/// Provides methods for changing state, retrieving current state, and acting based on status.
/// </summary>
public interface INonPlayableCharacter
{
    /// <summary>
    /// Method to change the NPC's state.
    /// </summary>
    /// <param name="newState">The new state to set for the NPC.</param>
    public void ChangeState(NPCStatus newState);

    /// <summary>
    /// Get the current state of the NPC.
    /// </summary>
    /// <returns>NPCStatus current status</returns>
    public NPCStatus GetCurrentState();

    /// <summary>
    /// Method to act based on the current status.
    /// </summary>
    public void ActOnStatus();
}

