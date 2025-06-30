/// <summary>
/// Interface for game save providers. Defines methods for saving, loading, resetting, and checking the existence of game save data.
/// </summary>
public interface IGameSaveProvider
{
    /// <summary>
    /// Saves the provided <see cref="GameSaveData"/> to persistent storage.
    /// </summary>
    /// <param name="data">The game save data to persist.</param>
    void SaveGame(GameSaveData data);

    /// <summary>
    /// Loads the <see cref="GameSaveData"/> from persistent storage.
    /// </summary>
    /// <returns>The loaded game save data, or null if not found or on error.</returns>
    GameSaveData LoadGame();

    /// <summary>
    /// Deletes the current save data from persistent storage.
    /// </summary>
    void ResetSave();

    /// <summary>
    /// Checks if a save file currently exists in persistent storage.
    /// </summary>
    /// <returns>True if a save file exists; otherwise, false.</returns>
    bool SaveExists();
}