using System.Threading.Tasks;

public interface IGameSaveProvider
{
    /// <summary>
    /// Exposes the current in-memory GameSaveData instance.
    /// </summary>
    GameSaveData CurrentSaveData { get; }

    /// <summary>
    /// Saves the provided game data to persistent storage.
    /// </summary>
    void SaveGame(GameSaveData data);

    /// <summary>
    /// Synchronously loads game data (blocking call; not recommended for gameplay).
    /// </summary>
    GameSaveData LoadGame();

    /// <summary>
    /// Asynchronously loads game data from persistent storage.
    /// </summary>
    Task<GameSaveData> LoadGameAsync();

    /// <summary>
    /// Resets the saved game data (for starting a new game, for example).
    /// </summary>
    void ResetSave();
}
