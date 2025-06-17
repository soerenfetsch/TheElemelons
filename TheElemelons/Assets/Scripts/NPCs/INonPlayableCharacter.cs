
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
/// Interface for all non-playable characters (NPCs).
/// Provides methods for changing state, querying current state, and acting based on state.
/// </summary>
public interface INonPlayableCharacter
{
    /// <summary>
    /// Changes the NPC's state to the specified new state.
    /// </summary>
    /// <param name="newState">The new state to set.</param>
    public void ChangeState(NPCStatus newState);

    /// <summary>
    /// Gets the current state of the NPC.
    /// </summary>
    /// <returns>The current <see cref="NPCStatus"/>.</returns>
    public NPCStatus GetCurrentState();

    /// <summary>
    /// Performs an action based on the NPC's current status.
    /// </summary>
    public void ActOnStatus();
}

