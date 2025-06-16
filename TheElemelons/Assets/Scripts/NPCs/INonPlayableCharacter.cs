
/// <summary>
/// Enum defining common NPC states.
/// </summary>
public enum NPCStatus
{
    /// <summary>
    /// NPC is idle (not moving or acting).
    /// </summary>
    Idle,
    /// <summary>
    /// NPC is walking or moving.
    /// </summary>
    Walking
}

/// <summary>
<<<<<<< Updated upstream
/// Interface for non-playable characters (NPCs) in the game.
/// Provides methods for changing state, retrieving current state, and acting based on status.
=======
/// Interface for all non-playable characters (NPCs).
/// Provides methods for changing state, querying current state, and acting based on state.
>>>>>>> Stashed changes
/// </summary>
public interface INonPlayableCharacter
{
    /// <summary>
<<<<<<< Updated upstream
    /// Method to change the NPC's state.
    /// </summary>
    /// <param name="newState">The new state to set for the NPC.</param>
    public void ChangeState(NPCStatus newState);

    /// <summary>
    /// Get the current state of the NPC.
=======
    /// Changes the NPC's state to the specified new state.
    /// </summary>
    /// <param name="newState">The new state to set.</param>
    public void ChangeState(NPCStatus newState);

    /// <summary>
    /// Gets the current state of the NPC.
>>>>>>> Stashed changes
    /// </summary>
    /// <returns>The current <see cref="NPCStatus"/>.</returns>
    public NPCStatus GetCurrentState();

    /// <summary>
<<<<<<< Updated upstream
    /// Method to act based on the current status.
=======
    /// Performs an action based on the NPC's current status.
>>>>>>> Stashed changes
    /// </summary>
    public void ActOnStatus();
}

