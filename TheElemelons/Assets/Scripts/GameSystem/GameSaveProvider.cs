using System;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks; // For Task.Run usage in coroutine-based methods

/// <summary>
/// Manages saving, loading, and resetting game data using JSON serialization.
/// Uses coroutines to perform file I/O operations asynchronously (off the main thread)
/// to prevent frame drops. Works with GameSaveData to persist game state.
/// </summary>
public class GameSaveProvider : MonoBehaviour, IGameSaveProvider
{
    [SerializeField] private string saveFileName = "gamesave.json";
    [SerializeField] private GameSaveData defaultData;

    /// <summary>
    /// Property that builds the full file path for saving and loading game data.
    /// </summary>
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);

    /// <summary>
    /// Holds a reference to the current runtime GameSaveData object.
    /// This can be used by the GameManager to update and save the game state.
    /// </summary>
    public GameSaveData CurrentSaveData { get; private set; }

    private void Awake()
    {
        // Initialize the current save data by cloning the default data if not already set.
        if (CurrentSaveData == null)
        {
            CurrentSaveData = Instantiate(defaultData);
        }
    }

    /// <summary>
    /// Saves the given game data asynchronously using a coroutine.
    /// </summary>
    /// <param name="data">The GameSaveData object to save.</param>
    public void SaveGame(GameSaveData data)
    {
        if (data == null)
        {
            Logger.LogError("Cannot save: Game data is null.");
            return;
        }
        StartCoroutine(SaveGameCoroutine(data));
    }

    /// <summary>
    /// Coroutine that serializes the GameSaveData object to JSON and writes it to disk off the main thread.
    /// </summary>
    private IEnumerator SaveGameCoroutine(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        bool finished = false;
        Exception exception = null;

        // Offload the file write to another thread.
        Task writeTask = Task.Run(() =>
        {
            try
            {
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finished = true;
        });

        while (!finished)
        {
            yield return null; // Yield back to main thread.
        }

        if (exception != null)
        {
            Logger.LogError("Error saving game: " + exception.Message);
        }
        else
        {
            Logger.Log("Game saved at: " + SaveFilePath);
        }
    }

    /// <summary>
    /// Asynchronously loads game data and returns a Task<GameSaveData>.
    /// </summary>
    public async Task<GameSaveData> LoadGameAsync()
    {
        try
        {
            if (!File.Exists(SaveFilePath))
            {
                Logger.LogWarning("No save file found; using default game data.");
                return Instantiate(defaultData);
            }

            string json = await Task.Run(() => File.ReadAllText(SaveFilePath));

            GameSaveData data = ScriptableObject.CreateInstance<GameSaveData>();
            JsonUtility.FromJsonOverwrite(json, data);
            Logger.Log("Game loaded successfully.");
            return data;
        }
        catch (Exception ex)
        {
            Logger.LogError("Error loading game: " + ex.Message);
            return Instantiate(defaultData);
        }
    }

    /// <summary>
    /// Synchronously loads game data (blocking call).
    /// </summary>
    public GameSaveData LoadGame()
    {
        var task = LoadGameAsync();
        task.Wait(); // Blocks main thread; not recommended for gameplay.
        return task.Result;
    }

    /// <summary>
    /// Replace the in-memory save with a freshly loaded one.
    /// </summary>
    public void SetCurrentSaveData(GameSaveData data)
    {
        if (data == null)
        {
            Logger.LogError("Cannot set current save data: data is null.");
            return;
        }
        CurrentSaveData = data;
    }

    /// <summary>
    /// Deletes the save file from disk.
    /// </summary>
    public void ResetSave()
    {
        // --- delete on-disk
        try
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                Logger.Log("Save file deleted.");
            }
            else
            {
                Logger.LogWarning("No save file found to delete.");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to delete save file: " + ex.Message);
        }

        // --- reset in-memory data to a fresh clone of our template
        if (defaultData != null)
        {
            CurrentSaveData = Instantiate(defaultData);
            Logger.Log("In-memory save data reset to defaults.");
        }
        else
        {
            Logger.LogError("No defaultData assigned! Cannot reset in-memory save.");
        }
    }
}

