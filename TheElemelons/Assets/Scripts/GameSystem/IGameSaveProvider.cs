/// <summary>
/// Interface for providing save, load, and reset functionality for game data.
/// Implementations handle the persistence and retrieval of <see cref="GameSaveData"/>.
/// </summary>
public interface IGameSaveProvider
{
    /// <summary>
    /// Saves the provided game data.
    /// </summary>
    /// <param name="data">The game data to save.</param>
    void SaveGame(GameSaveData data);

    /// <summary>
    /// Loads the game data.
    /// </summary>
    /// <returns>The loaded <see cref="GameSaveData"/> if available; otherwise, null.</returns>
    GameSaveData LoadGame();

    /// <summary>
    /// Deletes or resets the saved game data.
    /// </summary>
    void ResetSave();

    /// <summary>
    /// Checks if saved game data exists.
    /// </summary>
    /// <returns>True if a save exists; otherwise, false.</returns>
    bool SaveExists();
}