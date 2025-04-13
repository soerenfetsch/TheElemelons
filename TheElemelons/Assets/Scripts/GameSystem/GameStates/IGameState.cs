/// <summary>
/// Interface for game states.
/// Each state must implement logic for entering, exiting, and per-frame updates.
/// This abstraction allows future expansion and cleaner state management.
/// </summary>
public interface IGameState
{
    /// <summary>
    /// Called when the state is entered. Perform state-specific initialization here.
    /// </summary>
    void Enter();

    /// <summary>
    /// Called when exiting the state. Clean up or pause state-specific logic here.
    /// </summary>
    void Exit();

    /// <summary>
    /// Called each frame to update the state's logic.
    /// For many states, this might be empty.
    /// </summary>
    void UpdateState();
}
