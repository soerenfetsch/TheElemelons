using UnityEngine;

/// <summary>
/// Represents the active gameplay state where the player interacts with the game world.
/// </summary>
public class InGameState : IGameState
{
    /// <summary>
    /// Called when entering the in-game state.
    /// Initializes gameplay systems (e.g., enabling HUD, unpausing the game, etc.).
    /// </summary>
    public void Enter()
    {
        Logger.Log("Entering In-Game State");
        // Additional logic to activate gameplay UI, resume time, etc.
    }

    /// <summary>
    /// Called when exiting the in-game state.
    /// Performs any necessary cleanup before transitioning away from gameplay.
    /// </summary>
    public void Exit()
    {
        Logger.Log("Exiting In-Game State");
    }

    /// <summary>
    /// Called once per frame during gameplay.
    /// Includes any state-specific per-frame logic (if needed).
    /// </summary>
    public void UpdateState()
    {
        // Per-frame gameplay logic can be added here.
    }
}
