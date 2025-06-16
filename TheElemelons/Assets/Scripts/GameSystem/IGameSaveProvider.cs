/// <summary>
<<<<<<< Updated upstream
/// Interface for providing save, load, and reset functionality for game data.
/// Implementations handle the persistence and retrieval of <see cref="GameSaveData"/>.
=======
/// Interface for game save providers. Defines methods for saving, loading, resetting, and checking the existence of game save data.
>>>>>>> Stashed changes
/// </summary>
public interface IGameSaveProvider
{
    /// <summary>
<<<<<<< Updated upstream
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
=======
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
>>>>>>> Stashed changes
    /// </summary>
    void ResetSave();

    /// <summary>
<<<<<<< Updated upstream
    /// Checks if saved game data exists.
    /// </summary>
    /// <returns>True if a save exists; otherwise, false.</returns>
=======
    /// Checks if a save file currently exists in persistent storage.
    /// </summary>
    /// <returns>True if a save file exists; otherwise, false.</returns>
>>>>>>> Stashed changes
    bool SaveExists();
}