using UnityEngine;

/// <summary>
/// Abstract base class for interactable non-playable characters (NPCs).
/// Implements INonPlayableCharacter and provides state management for NPCs.
/// </summary>
public abstract class InteractableNPC : InteractableObject, INonPlayableCharacter
{
    // Current status of the NPC
    protected NPCStatus currentState = NPCStatus.Idle;

    /// <summary>
    /// Changes the NPC's state.
    /// </summary>
    /// <param name="newState">The new state to switch to.</param>
    public void ChangeState(NPCStatus newState)
    {
        currentState = newState;
    }

    /// <summary>
    /// Gets the current state of the NPC.
    /// </summary>
    /// <returns>The current state of the NPC.</returns> 
    public NPCStatus GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Defines how the NPC behaves based on its current state.
    /// </summary>
    public abstract void ActOnStatus();

    /// <summary>
    /// Sets the current state of the NPC.
    /// </summary>
    /// <param name="newState">The new state to set.</param>
    public void SetCurrentState(NPCStatus newState)
    {
        currentState = newState;
    }
}

